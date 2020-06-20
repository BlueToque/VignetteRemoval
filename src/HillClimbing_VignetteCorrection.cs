using OpenCvSharp;
using OpenCvSharp.Extensions;
using System.Drawing;
using VignetteRemoval.HillClimbing;

namespace VignetteRemoval
{
    /// <summary>
    /// Implements "Torsten Rohfling: Single-Image Vignetting Correction by Constrained Minimization of log-Intensity Entropy"
    /// 
    /// http://citeseerx.ist.psu.edu/viewdoc/download?doi=10.1.1.258.4780&rep=rep1&type=pdf 
    /// 
    /// based on code from https://github.com/dajuric/dot-devignetting
    /// </summary>
    public class Hill_VignetteCorrection
    {
        HillClimbingOptimization<DevignettingFunction, Mat<Vec3b>> m_optimizationAlg = null;

        public Image VignetteEstimate { get; private set; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="optimizeVignettingCentre">True to optimize spatial vignetting position, false otherwise.
        /// If set to false, the algorithm will perform significantly less number of steps. </param>
        public Hill_VignetteCorrection(bool optimizeVignettingCentre = true) => m_optimizeVignettingCentre = optimizeVignettingCentre;

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

            var endParamIdx = DevignettingFunction.PARAMETER_COUNT - 1;
            if (!m_optimizeVignettingCentre) endParamIdx -= 2; /*dX, dY*/

            m_optimizationAlg = new HillClimbingOptimization<DevignettingFunction, Mat<Vec3b>>(initialStep, stepReduction, finalStep, 0, endParamIdx);

            m_optimizationAlg.Initialize(DevignettingFunction.Empty, mat);

            while (!m_optimizationAlg.IsDone)
                m_optimizationAlg.MinimizeSingleStep(mat);

            Mat<Vec3b> result = mat.Clone();
            m_optimizationAlg.Function.Apply(result);

            VignetteEstimate = m_optimizationAlg.Function.GetGrayScale(mat.Width, mat.Height);

            return result.ToBitmap();
        }
    }
}