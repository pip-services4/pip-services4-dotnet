using PipServices4.Components.Context;
using System.Threading.Tasks;

namespace PipServices4.Components.Run
{
    /// <summary>
    /// Interface for components that require explicit opening and closing.
    /// 
    /// For components that perform opening on demand consider using
    /// IClosable interface instead.
    /// </summary>
    /// <example>
    /// <code>
    /// class MyPersistence: IOpenable 
    /// {
    ///     private object _client;
    ///     ...
    ///     public bool IsOpen()
    ///     {
    ///         return this._client != null;
    ///     }
    ///     
    ///     public void Open(IContext context)
    ///     {
    ///         if (this.isOpen())
    ///         {
    ///             return;
    ///         }
    ///         ...
    ///     }
    ///     
    ///     public void Close(IContext context)
    ///     {
    ///         if (this._client != null)
    ///         {
    ///             this._client.Close();
    ///             this._client = null;
    ///         }
    ///     }
    ///     
    ///     ...
    /// }
    /// </code>
    /// </example>
    /// See <see cref="Opener"/>
    public interface IOpenable : IClosable
    {
        /// <summary>
        /// Checks if the component is opened.
        /// </summary>
        /// <returns>true if the component has been opened and false otherwise.</returns>
        bool IsOpen();

        /// <summary>
        /// Opens the component.
        /// </summary>
        /// <param name="context">(optional) execution context to trace execution through call chain.</param>
        Task OpenAsync(IContext context);
    }
}
