using System.Collections.Generic;

namespace PipServices4.Azure.Controllers
{
    /// <summary>
    /// An interface that allows to integrate Azure Function services into Azure Function containers
    /// and connect their actions to the function calls.
    /// </summary>
    public interface IAzureFunctionController
    {
        /// <summary>
        /// Get all actions supported by the service.
        /// </summary>
        /// <returns>an array with supported actions.</returns>
        IList<AzureFunctionAction> GetActions();
    }
}