namespace VignetteRemoval.HillClimbing
{
    /// <summary>
    /// Optimization function interface.
    /// based on code from https://github.com/dajuric/dot-devignetting
    /// </summary>
    /// <typeparam name="TData">data type.</typeparam>
    public interface IFunction<TData>
    {
        /// <summary>
        /// Checks whether the function parameters are valid.
        /// </summary>
        /// <returns>True if the parameters are valid, false otherwise.</returns>
        bool IsValid();

        /// <summary>
        /// Evaluates the function using the provided data.
        /// </summary>
        /// <param name="data">Data.</param>
        /// <returns>Error value (if the minimization is used).</returns>
        float Evaluate(TData data);
    }

    /// <summary>
    /// Array indexer for the optimization function.
    /// </summary>
    public interface IParameterIndexer
    {
        /// <summary>
        /// Gets or sets the function parameter.
        /// </summary>
        /// <param name="paramIndex">parameter index. [0.. <see cref="ParameterCount"/> - 1]</param>
        /// <returns>Parameter value.</returns>
        float this[int paramIndex] { get; set; }

        /// <summary>
        /// Gets the parameter count.
        /// </summary>
        int ParameterCount { get; }
    }
}
