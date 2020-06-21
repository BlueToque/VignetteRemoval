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
using System.Collections.Generic;
using System.Drawing;
using CVSize = OpenCvSharp.Size;

namespace VignetteRemoval
{
	/// <summary>
	/// Adaptive Segmentation Image Variation Vignette Correction
	/// 
	/// This code is a port of the C++ version of the following
	/// https://github.com/GUOYI1/Vignetting_corrector
	/// 
	/// Based on the paper "Single-Image Vignetting Correction*,by Yuanjie Zheng,Stephen Lin,etc."
	/// https://www.microsoft.com/en-us/research/wp-content/uploads/2009/12/pami09zheng.pdf
	/// 
	/// </summary>
	public class ASIV_VignetteCorrection
	{
        public ASIV_VignetteCorrection() => Vignette = null;

        public ASIV_VignetteCorrection(List<double> vp) => Vignette = vp;

        #region public

        /// <summary>
        /// Process the image
        /// </summary>
        /// <param name="image"></param>
        /// <returns></returns>
        public Image Process(Image image)
		{
			// convert to OpenCV 
			var src = BitmapConverter.ToMat((Bitmap)image);

			int ht = src.Height;
			int wd = src.Width;
			int nChannels = src.Channels();

			double ratio = 1;
			if (wd > 75)
				ratio = 75.0 / wd;

			// resize the image
			int sht = (int)(ht * ratio + 0.5);
			int swd = (int)(wd * ratio + 0.5);

			Mat smallImage = src.Resize(new CVSize(swd, sht));

			// convert from image to gray
			Mat grayImage = nChannels == 3 ? smallImage.CvtColor(ColorConversionCodes.BGR2GRAY) : new Mat(smallImage);

			// create the image buffer
			byte[] imageBuffer = new byte[sht * swd];

			// get the indexer and copy the bytes to the image buffer
			var grayIndexer = new Mat<byte>(grayImage).GetIndexer();
			for (int j = 0; j < sht; j++)
				for (int i = 0; i < swd; i++)
					imageBuffer[j * swd + i] = grayIndexer[j, i];

			// Vignetting correction
			if (Vignette == null)
			{
				Vignette = new List<double>();
				if (!VignettingCorrectionUsingRG(imageBuffer, sht, swd, Vignette))
					return null;

				int n = Vignette.Count;
				for (int i = 0; i < n; i++)
					Vignette[i] = Math.Exp(Vignette[i]);

				double maxVr = Vignette[0];
				for (int i = 0; i < n; i++)
					Vignette[i] = Vignette[i] / maxVr;
			}

			int halfHt = (int)Math.Round(ht * 0.5);
			int halfWd = (int)Math.Round(wd * 0.5);
			int shalfHt = (int)Math.Round(sht * 0.5);
			int shalfWd = (int)Math.Round(swd * 0.5);

			#region apply the vignetting correction

			var mat3 = new Mat<Vec3b>(src);
			var indexer = mat3.GetIndexer();
			int nV = Vignette.Count;

			for (int j = 0; j < ht; j++)
			{
				for (int i = 0; i < wd; i++)
				{
					double cx = i - halfWd;
					double cy = j - halfHt;
					double radius = Math.Sqrt(cx * cx + cy * cy) + 0.5;
					radius *= ratio;

					// shd near interpolation
					double vValue;// = 1;
					int nR = (int)radius;

					if (nR == 0)
					{
						vValue = Vignette[0];
					}
					else if (nR < nV)
					{
						double dr = radius - nR;
						vValue = Vignette[nR - 1] * (1 - dr) + Vignette[nR] * dr;
					}
					else
					{
						vValue = Vignette[nV - 1];
					}

					//radius = max(0, min(nV-1, radius) );			
					//double scale = 1.0 / vp[radius];
					double scale = 1.0 / vValue;

					// multiply the RGB by the scale to change brightness
					int r = (int)(indexer[j, i][0] * scale);
					int g = (int)(indexer[j, i][1] * scale);
					int b = (int)(indexer[j, i][2] * scale);

					indexer[j, i] = new Vec3b((byte)Math.Min(255, r), (byte)Math.Min(255, g), (byte)Math.Min(255, b));
				}
			}

			#endregion

			#region Estimate the Correction

			Mat estimate_temp = new Mat(new CVSize(swd, sht), MatType.CV_8UC1);
			var est = new Mat<byte>(estimate_temp);
			var estIndexer = est.GetIndexer();

			for (int i = 0; i < estimate_temp.Height; i++)
			{
				//byte* data = ((byte*)Estimate_temp.Data.ToPointer()) + i * Estimate_temp.Step();
				for (int j = 0; j < estimate_temp.Width; j++)
				{
					int cx = i - shalfWd;
					int cy = j - shalfHt;
					int r = (int)(Math.Sqrt(cx * cx + cy * cy) + 0.5);
					if (r > 0 && r < nV + 1 && Vignette[r - 1] < 1)
						estIndexer[i, j] = (byte)Math.Round(255 * Vignette[r - 1]);
					else
						estIndexer[i, j] = 255;
				}
			}

			Mat estimate = estimate_temp.Resize(new CVSize(wd, ht));
			VignetteEstimate = estimate.ToBitmap();

			#endregion

			return mat3.ToBitmap();
		}

		/// <summary>
		/// The Vignette Estimate image
		/// </summary>
		public Bitmap VignetteEstimate { get; private set; }

		/// <summary>
		/// The vignette data 
		/// </summary>
		public List<double> Vignette { get; set; }
		
		#endregion

		#region private

		bool VignettingCorrectionUsingRG(byte[] pImage, int ht, int wd, List<double> vp)
		{
			int halfWd = (int)(wd * 0.5);
			int halfHt = (int)(ht * 0.5);

			int nRadius = (int)(Math.Sqrt(halfHt * halfHt + halfWd * halfWd) + 0.5 + 1);

			double[] weight = new double[ht * wd];
			for (int i = 0; i < ht * wd; i++)
				weight[i] = 1;

			double[] rgImage = new double[ht * wd];
			Array.Clear(rgImage, 0, rgImage.Length);

			double[] A = new double[nRadius];               //(double*)malloc(nRadius * sizeof(double));
			double[] At = new double[nRadius];              //(double*)malloc(nRadius * sizeof(double));
			double[] AtA = new double[nRadius * nRadius];   //(double*)malloc(nRadius * nRadius * sizeof(double));
			double[] sAtA = new double[nRadius * nRadius];  //(double*)malloc(nRadius * nRadius * sizeof(double));
			double[] sAtL = new double[nRadius];            //(double*)malloc(nRadius * sizeof(double));
			double[] result = new double[nRadius];          //(double*)malloc(nRadius * sizeof(double));
			double[] mB = new double[nRadius * nRadius];    //(double*)malloc(nRadius * nRadius * sizeof(double));
			Array.Clear(mB, 0, mB.Length);
			double[] mBt = new double[nRadius * nRadius];   //(double*)malloc(nRadius * nRadius * sizeof(double));
			Array.Clear(mBt, 0, mBt.Length);
			double[] mBtB = new double[nRadius * nRadius];  //(double*)malloc(nRadius * nRadius * sizeof(double));
			Array.Clear(mBtB, 0, mBtB.Length);

			//smooth constrait
			//lambda*(2*numPixels/numR)
			double lamdaS = 0.15 * 2 * (wd * ht) / nRadius;
			for (int i = 1; i < nRadius - 1; i++)
			{
				mB[i * nRadius + i] = -2;//*lamdaS;
				mB[i * nRadius + i - 1] = 1; //*lamdaS;
				mB[i * nRadius + i + 1] = 1; //*lamdaS;
			}

			Transpose(mB, mBt, nRadius, nRadius);
			Mult(mBt, mB, mBtB, nRadius, nRadius, nRadius);

			// calculate the radial gradients of image
			double shift = 1;
			double eps = 0.000001;
			for (int j = 1; j < ht; j++)
			{
				for (int i = 1; i < wd; i++)
				{
					int cx = i - halfWd;
					int cy = j - halfHt;

					// calculate the radius
					//int radius = (int)(Math.Sqrt(cx * cx + cy * cy) + 0.5);

					// calculate the gradient
					double dx = Math.Log(pImage[j * wd + i] + shift) - Math.Log(pImage[j * wd + i - 1] + shift);
					double dy = Math.Log(pImage[j * wd + i] + shift) - Math.Log(pImage[(j - 1) * wd + i] + shift);

					// calculate the radial gradient
					double rg = (cx * dx + cy * dy) / Math.Sqrt(cx * cx + cy * cy + eps);
					rgImage[j * wd + i] = rg;
				}
			}

			// weighted least square solution
			for (int iterIndex = 0; iterIndex < 5; iterIndex++)
			{
				Array.Clear(sAtA, 0, sAtA.Length);
				Array.Clear(sAtL, 0, sAtL.Length);

				for (int j = 1; j < ht; j++)
					for (int i = 1; i < wd; i++)
					{
						Array.Clear(A, 0, A.Length);
						Array.Clear(At, 0, At.Length);
						Array.Clear(AtA, 0, AtA.Length);

						//calculate the radius
						int cx = i - halfWd;
						int cy = j - halfHt;
						int radius = (int)(Math.Sqrt(cx * cx + cy * cy) + 0.5);

						double rg = rgImage[j * wd + i];

						// calculate the AtA of each pixel
						double right = 0;
						if (radius > 0 && radius < nRadius)
						{
							A[radius] = 1;
							A[radius - 1] = -1;
							right = rg;
						}

						for (int k = 0; k < nRadius; k++)
							At[k] = A[k];

						Mult(At, A, AtA, nRadius, 1, nRadius);

						//sum of AtA
						double w2 = weight[j * wd + i] * weight[j * wd + i];

						for (int k = 0; k < nRadius * nRadius; k++)
							sAtA[k] += AtA[k] * w2;

						for (int k = 0; k < nRadius; k++)
							sAtL[k] += At[k] * right * w2;
					}

				/////////////////////////////  adding constraints ///////////////////////
				// smooth constraint
				for (int i = 0; i < nRadius * nRadius; i++)
					sAtA[i] += lamdaS * lamdaS * mBtB[i];

				// vignetting value constraint, make them close to 1
				eps = 0.03;
				for (int i = 0; i < nRadius; i++)
					sAtA[i * nRadius + i] += eps;

				//////////////////////////////////////////////////////////////////////////

				Inverse_Matrix(sAtA, nRadius);
				Mult(sAtA, sAtL, result, nRadius, nRadius, 1);

				for (int i = 0; i < nRadius; i++)
					vp.Add(result[i]);

				// update weight
				double alpha = 0.6;
				for (int j = 1; j < ht; j++)
					for (int i = 1; i < wd; i++)
					{
						double rz; //radial gradient of image
						double rv; //radial gradient of vignetting paras

						int cx = i - halfWd;
						int cy = j - halfHt;

						//calculate the radius
						int radius = (int)(Math.Sqrt(cx * cx + cy * cy) + 0.5);
						radius = Math.Max(1, Math.Min(nRadius - 1, radius));

						//rv = log(vp[radius])-log(vp[radius-1]);
						rv = vp[radius] - vp[radius - 1];
						rz = rgImage[j * wd + i];

						double s1 = Math.Abs(rz - rv); //sqrt( (rz-rv)*(rz-rv) );
						double s2 = alpha * Math.Pow(s1, alpha - 1);
						weight[j * wd + i] = Math.Exp(-s1) * (1 - Math.Exp(-s2));
					}
			}

			return true;
		}

		/// <summary>
		/// Matrix  Multiplication
		/// </summary>
		/// <param name="m1"></param>
		/// <param name="m2"></param>
		/// <param name="result"></param>
		/// <param name="i_1"></param>
		/// <param name="j_12"></param>
		/// <param name="j_2"></param>
		void Mult(double[] m1, double[] m2, double[] result, int i_1, int j_12, int j_2)
		{
			for (int i = 0; i < i_1; i++)
				for (int j = 0; j < j_2; j++)
				{
					result[i * j_2 + j] = 0.0;

					for (int k = 0; k < j_12; k++)
						result[i * j_2 + j] += m1[i * j_12 + k] * m2[j + k * j_2];
				}
		}

		/// <summary>
		/// Invert Matrix 
		/// </summary>
		/// <param name="m1"></param>
		/// <param name="n"></param>
		/// <returns></returns>
		bool Inverse_Matrix(double[] m1, int n)
		{
			int[] i_s = new int[n];
			int[] js = new int[n];
			int i, j, k, l, u, v;
			double temp, max_v;

			for (k = 0; k < n; k++)
			{
				max_v = 0.0;
				for (i = k; i < n; i++)
				{
					for (j = k; j < n; j++)
					{
						temp = Math.Abs(m1[i * n + j]);
						if (temp > max_v)
						{
							max_v = temp;
							i_s[k] = i; js[k] = j;
						}
					}
				}

				// inverse not available
				if (max_v == 0.0) return false;

				if (i_s[k] != k)
				{
					for (j = 0; j < n; j++)
					{
						u = k * n + j;
						v = i_s[k] * n + j;
						temp = m1[u];
						m1[u] = m1[v];
						m1[v] = temp;
					}
				}

				if (js[k] != k)
				{
					for (i = 0; i < n; i++)
					{
						u = i * n + k;
						v = i * n + js[k];
						temp = m1[u];
						m1[u] = m1[v];
						m1[v] = temp;
					}
				}

				l = k * n + k;
				m1[l] = 1.0 / m1[l];
				for (j = 0; j < n; j++)
				{
					if (j != k)
					{
						u = k * n + j;
						m1[u] *= m1[l];
					}
				}

				for (i = 0; i < n; i++)
				{
					if (i != k)
					{
						for (j = 0; j < n; j++)
						{
							if (j != k)
							{
								u = i * n + j;
								m1[u] -= m1[i * n + k] * m1[k * n + j];
							}
						}
					}
				}

				for (i = 0; i < n; i++)
				{
					if (i != k)
					{
						u = i * n + k;
						m1[u] *= -m1[l];
					}
				}

			}

			for (k = n - 1; k >= 0; k--)
			{
				if (js[k] != k)
				{
					for (j = 0; j < n; j++)
					{
						u = k * n + j; v = js[k] * n + j;
						temp = m1[u]; m1[u] = m1[v]; m1[v] = temp;
					}
				}

				if (i_s[k] != k)
				{
					for (i = 0; i < n; i++)
					{
						u = i * n + k; v = i * n + i_s[k];
						temp = m1[u]; m1[u] = m1[v]; m1[v] = temp;
					}
				}
			}

			return true;
		}

		/// <summary>
		/// Matrix Transpose
		/// </summary>
		/// <param name="m1"></param>
		/// <param name="m2"></param>
		/// <param name="m"></param>
		/// <param name="n"></param>
		void Transpose(double[] m1, double[] m2, int m, int n)
		{
			for (int i = 0; i < m; i++)
				for (int j = 0; j < n; j++)
					m2[j * m + i] = m1[i * n + j];
		}

		#endregion
	}

}
