using OpenCvSharp;
using OpenCvSharp.Extensions;
using System;
using System.Drawing;
using System.Linq;
using CVSize = OpenCvSharp.Size;

namespace VignetteRemoval.HillClimbing
{
    /// <summary>
    /// De-vignetting polynomial function (1 + a*r^2 + b*r^4 + c*r^6) where r is radius [0..1].
    /// </summary>
    class DevignettingFunction : IFunction<Mat<Vec3b>>, IParameterIndexer
    {
        /// <summary> Gets parameter count. </summary>
        public static readonly int PARAMETER_COUNT = 3 /*poly params*/ + 2 /*spatial offset*/;

        const int HISTOGRAM_SIZE = 256; //depends on Step (see Optimization.c) binCount = max 255 * step

        const int LOG2_256 = 8;

        /// <summary> Gets empty de-vignetting function. </summary>
        public static readonly DevignettingFunction Empty = new DevignettingFunction { A = 0, B = 0, C = 0, DeltaX = 0, DeltaY = 0 };

        /// <summary>
        /// The maximum brightness multiplication (the factor which dictates how much pixels can be changed).
        /// </summary>
        public static readonly float MAX_BRIGHTNESS_MULTIPLICATION = 3;

        /// <summary>
        /// Gets or sets the 'a' factor for the polynomial: (1 + a*r^2 + b*r^4 + c*r^6).
        /// </summary>
        public float A { get; set; }

        /// <summary>
        /// Gets or sets the 'b' factor for the polynomial: (1 + a*r^2 + b*r^4 + c*r^6).
        /// </summary>
        public float B { get; set; }

        /// <summary>
        /// Gets or sets the 'c' factor for the polynomial: (1 + a*r^2 + b*r^4 + c*r^6).
        /// </summary>
        public float C { get; set; }

        /// <summary> Gets or sets horizontal offset from the image center. </summary>
        public float DeltaX { get; set; }

        /// <summary> Gets or sets vertical offset from the image center. </summary>
        public float DeltaY { get; set; }

        #region Utility functions

        private static byte ClampToByte(int val) => (byte)(val > Byte.MaxValue ? Byte.MaxValue : val);

        static int Log2i(int value)
        {
            int log2 = -1; //return -1 if value == 0 (zero value should be forbidden)

            while (value != 0)
            {
                value >>= 1;
                log2++;
            }

            return log2;
        }

        static float GetGainAt(DevignettingFunction f, int x, int y, int width, int height)
        {
            float centerX = width / 2 + f.DeltaX;
            float centerY = height / 2 + f.DeltaY;

            float radiusSquared = (x - centerX) * (x - centerX) + (y - centerY) * (y - centerY);
            radiusSquared /= (centerX * centerX + centerY * centerY);

            //gain = 1 + a * r^2 + b * r^4 + c * r^6 
            float gain = 1 + (radiusSquared * (f.A + radiusSquared * (f.B + radiusSquared * f.C)));
            return gain;
        }

        static void UpdateHistogram(int[] histogram, int value)
        {
            int MUL_FACTOR = (int)((float)(HISTOGRAM_SIZE - 1) / LOG2_256 / MAX_BRIGHTNESS_MULTIPLICATION + 1);
            int idx = MUL_FACTOR * Log2i(value + 1); //scale value
            histogram[idx]++;
        }

        static float CalculateEntropy(int[] histogram)
        {
            int sum = histogram.Sum();

            float entropy = 0;
            for (int i = 0; i < histogram.Length; i++)
            {
                if (histogram[i] == 0) continue;

                float p = (float)histogram[i] / sum;
                entropy += p * (float)(Math.Log(p) / Math.Log(2));
            }

            return -entropy; //convert to positive value
        }

        #endregion

        /// <summary>
        /// Applies the de-vignetting function to the specified color image.
        /// </summary>
        /// <param name="image">Image to correct.</param>
        public Image GetGrayScale(int width, int height)
        {
            Mat<byte> image = new Mat<byte>(width, height);
            image.SetTo(128);

            var indexer = image.GetIndexer();
            var rows = image.Rows;
            var cols = image.Cols;
            for (int r = 0; r < rows; r++)
            {
                for (int c = 0; c < cols; c++)
                {
                    float gain = GetGainAt(this, c, r, cols, rows);
                    byte data = indexer[r, c];
                    data = ClampToByte((int)(data * gain));
                    indexer[r, c] = data;
                }
            }
            
            return image.Resize(new CVSize(512, 512)).ToBitmap();
        }

        /// <summary>
        /// Applies the de-vignetting function to the specified color image.
        /// </summary>
        /// <param name="image">Image to correct.</param>
        public void Apply(Mat<Vec3b> image)
        {
            var indexer = image.GetIndexer();
            var rows = image.Rows;
            var cols = image.Cols;
            for (int r = 0; r < rows; r++)
            {
                for (int c = 0; c < cols; c++)
                {
                    float gain = GetGainAt(this, c, r, image.Cols, image.Rows);
                    Vec3b data = indexer[r, c];
                    data[0] = ClampToByte((int)(data[0] * gain));
                    data[1] = ClampToByte((int)(data[1] * gain));
                    data[2] = ClampToByte((int)(data[2] * gain));
                    indexer[r, c] = data;
                }
            }
        }

        #region IFunction

        /// <summary>
        /// Checks whether the function parameters are valid.
        /// <para>The polynomial coefficients are verified whether they construct monotonically increasing polynomial or not.</para>
        /// </summary>
        /// <returns>True if the parameters are valid, false otherwise.</returns>
        public bool IsValid()
        {
            // radius is assumed to be maximum (or 1)
            if ((1 + A + B + C) > MAX_BRIGHTNESS_MULTIPLICATION) 
                return false;

            if (C == 0)
            {
                if (A < 0 || (A + 2 * B < 0))
                    return false;
            }
            else
            {
                float D = 4 * B * B - 12 * A * C;
                float qMins = (-2 * B - (float)Math.Sqrt(D)) / (6 * C);
                float qPlus = (-2 * B + (float)Math.Sqrt(D)) / (6 * C);

                if (C < 0)
                {
                    if (D >= 0)
                    {
                        if (qMins > 0 || qPlus < 1)
                            return false;
                    }
                    else
                        return false;
                }
                else //if(p.C > 0) case where p.C == 0 is covered at the beginning 
                {
                    if (D >= 0)
                    {
                        if (!((qMins <= 0 && qPlus <= 0) || (qMins >= 1 && qPlus >= 1)))
                            return false;
                    }
                }
            }

            return true;
        }

        /// <summary>
        /// Evaluates the function using the provided image.
        /// </summary>
        /// <param name="image">Color image.</param>
        /// <returns>Entropy value. Smaller entropy is preferred.</returns>
        public float Evaluate(Mat<Vec3b> image)
        {
            var histogram = new int[HISTOGRAM_SIZE];

            var indexer = image.GetIndexer();
            var rows = image.Rows;
            var cols = image.Cols;
            for (int r = 0; r < rows; r++)
            {
                for (int c = 0; c < cols; c++)
                {
                    Vec3b data = indexer[r, c];
                    float intensity = 0.2126f * data[0] + 0.7152f * data[1] + 0.0722f * data[2];
                    float gain = GetGainAt(this, c, r, cols, rows);
                    UpdateHistogram(histogram, (int)(gain * intensity));
                }
            }

            return CalculateEntropy(histogram);
        }

        #endregion

        #region IFunctionIndexer

        /// <summary> Gets the parameter count. </summary>
        public int ParameterCount => PARAMETER_COUNT;

        /// <summary>
        /// Gets or sets the function parameter.
        /// </summary>
        /// <param name="paramIndex">parameter index. [0.. <see cref="ParameterCount"/> - 1]</param>
        /// <returns>Parameter value.</returns>
        public float this[int paramIndex]
        {
            get
            {
                switch (paramIndex)
                {
                    case 0: return A;
                    case 1: return B;
                    case 2: return C;
                    case 3: return DeltaX;
                    case 4: return DeltaY;
                    default: throw new IndexOutOfRangeException();
                }
            }
            set
            {
                switch (paramIndex)
                {
                    case 0: A = value; break;
                    case 1: B = value; break;
                    case 2: C = value; break;
                    case 3: DeltaX = value; break;
                    case 4: DeltaY = value; break;
                    default: throw new IndexOutOfRangeException();
                }
            }
        }

        #endregion
    }
}
