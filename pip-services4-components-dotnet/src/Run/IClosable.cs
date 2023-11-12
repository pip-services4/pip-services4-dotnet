using PipServices4.Components.Context;
using System.Threading.Tasks;

namespace PipServices4.Components.Run
{
    /// <summary>
    /// Interface for components that require explicit closure.
    /// 
    /// For components that require opening as well as closing
    /// use IOpenable interface instead.
    /// </summary>
    /// <example>
    /// <code>
    /// class MyConnector: IClosable 
    /// {
    ///     private object _client = null;
    ///     
    ///     ... // The _client can be lazy created
    ///     
    ///     public void Close(IContext context)
    ///     {
    ///         if (this._client != null)
    ///         {   
    ///             this._client.Close();
    ///             this._client = null;
    ///         }
    ///     }
    /// }
    /// </code>
    /// </example>
    /// See <see cref="IOpenable"/>, <see cref="Closer"/>
    public interface IClosable
    {
        /// <summary>
        /// Closes component and frees used resources.
        /// </summary>
        /// <param name="context">(optional) execution context to trace execution through call chain.</param>
        Task CloseAsync(IContext context);
    }
}
