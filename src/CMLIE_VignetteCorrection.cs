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
using System.Drawing;
using VignetteRemoval.HillClimbing;

namespace VignetteRemoval
{
    /// <summary>
    /// CMLIE
    /// Implements "Torsten Rohfling: Single-Image Vignetting Correction by Constrained Minimization of log-Intensity Entropy"
    /// 
    /// http://citeseerx.ist.psu.edu/viewdoc/download?doi=10.1.1.258.4780&rep=rep1&type=pdf 
    /// 
    /// based on code from https://github.com/dajuric/dot-devignetting
    /// </summary>
    public class CMLIE_VignetteCorrection
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="optimizeVignettingCentre">True to optimize spatial vignetting position, false otherwise.
        /// If set to false, the algorithm will perform significantly less number of steps. </param>
        public CMLIE_VignetteCorrection(bool optimizeVignettingCentre = true) => m_optimizeVignettingCentre = optimizeVignettingCentre;

        HillClimbingOptimization m_optimizationAlg;

        public Image VignetteEstimate { get; private set; }

        readonly bool m_optimizeVignettingCentre = true;

        /// <summary>
        /// Creates and initializes new de-vignetting algorithm.
        /// </summary>
        /// <param name="image">Original image.</param>
        public Image Process(Image image)
        {
            Mat<Vec3b> mat = new Mat<Vec3b>(BitmapConverter.ToMat((Bitmap)image));

            var initialStep = new float[] { 5, 5, 5, image.Width / 4, image.Height / 4 };
            var stepReduction = new float[] { 0.5f, 0.5f, 0.5f, 0.5f, 0.5f };
            var finalStep = new float[] { 1f / 256, 1f / 256, 1f / 256, 1, 1 };
            var endParamIdx = Function.PARAMETER_COUNT - 1;

            if (!m_optimizeVignettingCentre) endParamIdx -= 2; /*dX, dY*/

            m_optimizationAlg = new HillClimbingOptimization(initialStep, stepReduction, finalStep, 0, endParamIdx);

            m_optimizationAlg.Initialize(Function.Empty, mat);

            while (!m_optimizationAlg.IsDone)
                m_optimizationAlg.MinimizeSingleStep(mat);

            Mat<Vec3b> result = mat.Clone();
            m_optimizationAlg.Function.Apply(result);

            VignetteEstimate = m_optimizationAlg.Function.GetGrayScale(mat.Width, mat.Height);

            return result.ToBitmap();
        }
    }
}