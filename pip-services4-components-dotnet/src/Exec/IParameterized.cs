namespace PipServices4.Components.Exec
{
    /// <summary>
    /// Interface for components that require execution parameters.
    /// </summary>
    public interface IParameterized
    {
        /// <summary>
        /// Sets execution parameters.
        /// </summary>
        /// <param name="parameters">execution parameters.</param>
        void SetParameters(Parameters parameters);
    }
}
