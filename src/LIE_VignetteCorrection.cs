/*
	MIT License

	Copyright (c) 2020 BlueToque Software

	Permission is hereby granted, free of charge, to any person obtaining a copy
	of this software and associated documentation files (the "Software"), to deal
	in the Software without restriction, including without limitation the rights
	to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
	copies of the Software, and to permit persons to whom the Software is
	furnished to do so, subject to the following conditions:

	The above copyright notice and this permission notice shall be included in all
	copies or substantial portions of the Software.

	THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
	IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
	FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
	AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
	LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
	OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
	SOFTWARE.
*/
using OpenCvSharp;
using OpenCvSharp.Extensions;
using System;
using System.Drawing;

namespace VignetteRemoval
{
    /// <summary>
    /// Log Intensity Entropy Vignette Correction
    /// 
    /// C# implementation of "Revisiting Image Vignetting Correction by Constrained Minimization of log-Intensity Entropy".
    /// https://www.researchgate.net/publication/300786398_Revisiting_Image_Vignetting_Correction_by_Constrained_Minimization_of_Log-Intensity_Entropy
    /// 
    /// based on the c++ implementation: 
    /// https://github.com/HJCYFY/Vignetting-Correction
    /// </summary>
    public class LIE_VignetteCorrection
    {
        public Image Process(Image image)
        {
            Mat img = BitmapConverter.ToMat((Bitmap)image);

            Mat imgGray = img.CvtColor(ColorConversionCodes.BGR2GRAY);

            Mat<byte> aa = new Mat<byte>(imgGray);

            float a = 0, b = 0, c = 0;
            float a_min = 0, b_min = 0, c_min = 0;
            float delta = 8;
            float Hmin = CalH(a, b, c, imgGray);

            while (delta > (1 / 256.0f))
            {
                float a_temp = a + delta;
                if (Check(a_temp, b, c))
                {
                    float H = CalH(a_temp, b, c, imgGray);
                    if (Hmin > H)
                    {
                        a_min = a_temp;
                        b_min = b;
                        c_min = c;
                        Hmin = H;
                    }
                }
                a_temp = a - delta;
                if (Check(a_temp, b, c))
                {
                    float H = CalH(a_temp, b, c, imgGray);
                    if (Hmin > H)
                    {
                        a_min = a_temp;
                        b_min = b;
                        c_min = c;
                        Hmin = H;
                    }
                }
                float b_temp = b + delta;
                if (Check(a, b_temp, c))
                {
                    float H = CalH(a, b_temp, c, imgGray);
                    if (Hmin > H)
                    {
                        a_min = a;
                        b_min = b_temp;
                        c_min = c;
                        Hmin = H;
                    }
                }
                b_temp = b - delta;
                if (Check(a, b_temp, c))
                {
                    float H = CalH(a, b_temp, c, imgGray);
                    if (Hmin > H)
                    {
                        a_min = a;
                        b_min = b_temp;
                        c_min = c;
                        Hmin = H;
                    }
                }
                float c_temp = c + delta;
                if (Check(a, b, c_temp))
                {
                    float H = CalH(a, b, c_temp, imgGray);
                    if (Hmin > H)
                    {
                        a_min = a;
                        b_min = b;
                        c_min = c_temp;
                        Hmin = H;
                    }
                }
                c_temp = c - delta;
                if (Check(a, b, c_temp))
                {
                    float H = CalH(a, b, c_temp, imgGray);
                    if (Hmin > H)
                    {
                        a_min = a;
                        b_min = b;
                        c_min = c_temp;
                        Hmin = H;
                    }
                }
                delta /= 2.0f;
            }

            // convert original to LAB colour?
            var lab = img.CvtColor(ColorConversionCodes.BGR2Lab);

            // get the L value as a mat
            Mat[] lab_planes = lab.Split();

            Mat<byte> result = ApplyFunction(new Mat<byte>(lab_planes[0]), a_min, b_min, c_min);

            result.CopyTo(lab_planes[0]);
            Cv2.Merge(lab_planes, lab);

            Mat rgb = lab.CvtColor(ColorConversionCodes.Lab2BGR);

            aa.Release();
            return rgb.ToBitmap();
        }

        private static Mat<byte> ApplyFunction(Mat<byte> aa, float a_min, float b_min, float c_min)
        {
            int rows = aa.Rows;
            int cols = aa.Cols;
            Mat<byte> result = new Mat<byte>(rows, cols);

            float c_x = cols / 2.0f;
            float c_y = rows / 2.0f;
            float d = (float)Math.Sqrt(c_x * c_x + c_y * c_y);

            var aaIndexer = aa.GetIndexer();
            var resultIndexer = result.GetIndexer();
            for (int row = 0; row < rows; ++row)
            {
                for (int col = 0; col < cols; ++col)
                {
                    float r = (float)Math.Sqrt((row - c_y) * (row - c_y) + (col - c_x) * (col - c_x)) / d;
                    float r2 = r * r;
                    float r4 = r2 * r2;
                    float r6 = r2 * r2 * r2;
                    float g = 1 + a_min * r2 + b_min * r4 + c_min * r6;

                    // this will cause overflow 
                    // ToDo: The image should be normalized to the original brightness
                    resultIndexer[row, col] = (byte)Math.Round(aaIndexer[row, col] * g);
                    if (resultIndexer[row, col] > 255)
                        resultIndexer[row, col] = 255;
                    else if (resultIndexer[row, col] < 0)
                        resultIndexer[row, col] = 0;
                }
            }

            return result;
        }

        bool Check(float a, float b, float c)
        {
            if ((a > 0) && (b == 0) && (c == 0))
                return true;
            if (a >= 0 && b > 0 && c == 0)
                return true;
            if (c == 0 && b < 0 && -a <= 2 * b)
                return true;
            if (c > 0 && b * b < 3 * a * c)
                return true;
            if (c > 0 && b * b == 3 * a * c && b >= 0)
                return true;
            if (c > 0 && b * b == 3 * a * c && -b >= 3 * c)
                return true;
            float q_p = (float)(-2 * b + Math.Sqrt(4 * b * b - 12 * a * c)) / (6 * c);
            if (c > 0 && b * b > 3 * a * c && q_p <= 0)
                return true;
            float q_d = (float)(-2 * b - Math.Sqrt(4 * b * b - 12 * a * c)) / (6 * c);
            if (c > 0 && b * b > 3 * a * c && q_d >= 1)
                return true;
            if (c < 0 && b * b > 3 * a * c && q_p >= 1 && q_d <= 0)
                return true;
            return false;
        }

        float CalH(float a, float b, float c, Mat grayImg)
        {
            Mat<float> grayFloatImg = new Mat<float>(grayImg.Rows,grayImg.Cols);
            int rows = grayImg.Rows;
            int cols = grayImg.Cols;

            float c_x = (float)(cols / 2.0);
            float c_y = (float)(rows / 2.0);
            float d = (float)Math.Sqrt(c_x * c_x + c_y * c_y);

            var grayIndexer = new Mat<byte>( grayImg).GetIndexer();
            var grayFloatIndexer = grayFloatImg.GetIndexer();

            for (int row = 0; row < rows; ++row)
            {
                //var data = grayImg.Row(row).GetGenericIndexer<byte>();
                //var value = grayFloatImg.Row(row).GetGenericIndexer<float>();
                for (int col = 0; col < cols; ++col)
                {
                    float r = (float)Math.Sqrt((row - c_y) * (row - c_y) + (col - c_x) * (col - c_x)) / d;
                    float r2 = r * r;
                    float r4 = r2 * r2;
                    float r6 = r2 * r2 * r2;
                    float g = 1 + a * r2 + b * r4 + c * r6;
                    grayFloatIndexer[row, col] = grayIndexer[row, col] * g;
                    //value[col] = data[col] * g;
                }
            }

            var logImg = new Mat<float>(rows,cols);
            var logIndexer = logImg.GetIndexer();
            for (int row = 0; row < rows; ++row)
                for (int col = 0; col < cols; ++col)
                    logIndexer[row,col] = (float)(255 * Math.Log(1 + grayFloatIndexer[row, col]) / 8);

            float[] histogram = new float[256];
            for (int row = 0; row < rows; ++row)
            {
                for (int col = 0; col < cols; ++col)
                {
                    int k_d = (int)Math.Floor(logIndexer[row,col]);
                    int k_u = (int)Math.Ceiling(logIndexer[row, col]);
                    histogram[k_d] += (1 + k_d - logIndexer[row, col]);
                    histogram[k_u] += (k_u - logIndexer[row, col]);
                }
            }

            float[] tempHist = new float[256 + 2 * 4];            //    SmoothRadius = 4
            tempHist[0] = histogram[4]; 
            tempHist[1] = histogram[3];
            tempHist[2] = histogram[2]; 
            tempHist[3] = histogram[1];
            tempHist[260] = histogram[254]; 
            tempHist[261] = histogram[253];
            tempHist[262] = histogram[252]; 
            tempHist[263] = histogram[251];
            Array.Copy(histogram, 0, tempHist, 4, 256);
            //memcpy(TempHist + 4, histogram, 256 * sizeof(float));

            //  smooth
            for (int X = 0; X < 256; X++)
                histogram[X] = (tempHist[X] + 2 * tempHist[X + 1] + 3 * tempHist[X + 2] + 4 * tempHist[X + 3] + 5 * tempHist[X + 4] + 4 * tempHist[X + 5] + 3 * tempHist[X + 6] + 2 * tempHist[X + 7]) + tempHist[X + 8] / 25.0f;

            float sum = 0;
            for (int i = 0; i < 256; ++i)
                sum += histogram[i];
            float H = 0, pk;
            for (int i = 0; i < 256; ++i)
            {
                pk = histogram[i] / sum;
                if (pk != 0)
                    H += (float)(pk * Math.Log(pk));
            }

            grayFloatImg.Release();
            logImg.Release();

            return -H;
        }
    }
}