using PipServices4.Components.Context;
using System.Threading.Tasks;

namespace PipServices4.Components.Run
{
    /// <summary>
    /// Interface for components that should clean their state.
    /// 
    /// Cleaning state most often is used during testing.
    /// But there may be situations when it can be done in production.
    /// </summary>
    /// <example>
    /// <code>
    /// class MyObjectWithState: ICleanable 
    /// {
    ///     var _state = new Object[]{};
    ///     ...
    ///     public void Clear(IContext context)
    ///     {
    ///         this._state = new Object[] { };
    ///     }
    /// }
    /// </code>
    /// </example>
    public interface ICleanable
    {
        /// <summary>
        /// Clears component state.
        /// </summary>
        /// <param name="context">(optional) execution context to trace execution through call chain.</param>
        Task ClearAsync(IContext context);
    }
}
