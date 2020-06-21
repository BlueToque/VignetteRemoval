using OpenCvSharp;
using System;

namespace VignetteRemoval.HillClimbing
{
    /// <summary>
    /// Hill-climbing algorithm.
    /// based on code from https://github.com/dajuric/dot-devignetting
    /// </summary>
    /// <typeparam name="TFunction">Optimization function type.</typeparam>
    /// <typeparam name="TData">Data type for the provider function.</typeparam>
    public class HillClimbingOptimization//<TFunction, TData> where TFunction : IFunction<TData>, IParameterIndexer
    {
        /// <summary>
        /// Creates new hill-climbing algorithm.
        /// </summary>
        /// <param name="initialStep">Initial step for each parameter.</param>
        /// <param name="stepReduction">Step reduction for each parameter.</param>
        /// <param name="finalStep">Final step for each parameter.</param>
        /// <param name="startParamIdx">Starting index of an parameter to optimize.</param>
        /// <param name="endParamIdx">Ending index of an parameter to optimize.</param>
        public HillClimbingOptimization(float[] initialStep, float[] stepReduction, float[] finalStep, int startParamIdx, int endParamIdx)
        {
            InitialStep = initialStep;
            StepReduction = stepReduction;
            FinalStep = finalStep;
            StartParamIdx = startParamIdx;
            EndParamIdx = endParamIdx;
        }

        float[] m_step;
        int m_prevIterId;
        int m_delta;
        float m_bestValue;

        int m_updateIndex;
        float m_updateParam;
        int m_maxParamIndex;
        int m_currentParamIndex;

        bool m_isImproved;

        /// <summary>
        /// Initializes the optimization algorithm by performing initial function evaluation to set the initial state.
        /// </summary>
        public void Initialize(Function initialFunction, Mat<Vec3b> data)
        {
            Function = initialFunction;

            m_step = (float[])InitialStep.Clone();
            m_prevIterId = -1;
            m_delta = -1;
            m_bestValue = initialFunction.Evaluate(data);

            m_updateIndex = -1;
            m_updateParam = Single.PositiveInfinity;
            m_maxParamIndex = StartParamIdx;
            m_currentParamIndex = StartParamIdx;

            m_isImproved = false;
            IsDone = false;
        }


        private bool MinimizeSingleStepDeltaOnly1D(Mat<Vec3b> data)
        {
            //----- for delta = [-1, +1] -----
            if (m_delta <= 1)
            {
                //----- core -----
                float oldP = Function[m_currentParamIndex];
                Function[m_currentParamIndex] += m_step[m_currentParamIndex] * m_delta;

                int thisIterId = (m_currentParamIndex + 1) * m_delta;
                bool isInBounds = Function.IsValid();

                if (m_prevIterId != thisIterId && isInBounds)
                {
                    float value = Function.Evaluate(data);

                    if (value < m_bestValue)
                    {
                        m_isImproved = true;
                        m_prevIterId = thisIterId;
                        m_bestValue = value;

                        m_updateIndex = m_currentParamIndex;
                        m_updateParam = Function[m_currentParamIndex];
                    }
                }

                Function[m_currentParamIndex] = oldP;
                //----- core -----

                m_delta += 2; //(-1, +1, -1, +1...)

                if (m_delta <= 1)
                    return false;
                else
                    m_delta = -1;
            }
            //----- for delta = [-1, +1] -----

            return true;
        }

        /// <summary>
        /// Performs single iteration of minimization using the given data.
        /// </summary>
        /// <param name="data">Data to be passed to the evaluating function.</param>
        /// <returns>
        /// True if the optimization has finished, false otherwise.
        /// <para>See <see cref="IsDone"/>.</para>
        /// </returns>
        public bool MinimizeSingleStep(Mat<Vec3b> data)
        {
            //incremental optimization by increasing polynomial degree
            //----- for maxParamIdx = 1: length(initialFuncParams) -----
            if (m_maxParamIndex <= EndParamIdx)
            {
                //----- while step >= finalStep -----
                if (m_step[m_currentParamIndex] >= FinalStep[m_currentParamIndex])
                {
                    // repeat hill - climbing procedure if we improving the result
                    //	----- for i = 1: maxParamIdx -----
                    if (m_currentParamIndex <= m_maxParamIndex)
                    {
                        if (!MinimizeSingleStepDeltaOnly1D(data)) return false;
                        m_currentParamIndex++;

                        if (m_currentParamIndex <= m_maxParamIndex)
                            return false;
                        else
                        {
                            m_currentParamIndex = StartParamIdx;
                            m_prevIterId = 0;
                        }
                    }
                    //----- for i = 1: maxParamIdx -----

                    if (m_isImproved) //after all parameters are tried out
                    {
                        Function[m_updateIndex] = m_updateParam;
                        m_isImproved = false;
                        return false;
                    }
                    //----- while isImproved -----

                    m_step[m_currentParamIndex] *= StepReduction[m_currentParamIndex];

                    if (m_step[m_currentParamIndex] >= FinalStep[m_currentParamIndex])
                        return false;
                    else
                        m_step[m_currentParamIndex] = InitialStep[m_currentParamIndex];
                }
                //----- while step >= finalStep -----

                m_maxParamIndex++;

                if (m_maxParamIndex < Function.ParameterCount)
                    return false; //do not return counter back to 1
            }

            IsDone = true;
            return true;
        }

        /// <summary> Initial step size. </summary>
        public float[] InitialStep { get; private set; }

        /// <summary> Step reduction factor. </summary>
        public float[] StepReduction { get; private set; }

        /// <summary> Final step size. </summary>
        public float[] FinalStep { get; private set; }

        /// <summary>
        /// index for the starting optimizing parameter.
        /// </summary>
        public int StartParamIdx { get; private set; }

        /// <summary>
        /// index for the last parameter to optimize.
        /// </summary>
        public int EndParamIdx { get; private set; }

        /// <summary>
        /// Returns true if the optimization is finished, false otherwise.
        /// </summary>
        public bool IsDone { get; private set; }

        /// <summary> current function state. </summary>
        public Function Function { get; private set; }
    }
}
