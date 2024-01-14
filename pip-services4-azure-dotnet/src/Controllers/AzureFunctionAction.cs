using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PipServices4.Data.Validate;
using System;
using System.Threading.Tasks;

namespace PipServices4.Azure.Controllers
{
    public class AzureFunctionAction
    {
        /// <summary>
        /// Command to call the action
        /// </summary>
        public string Cmd { get; set; }

        /// <summary>
        /// Schema to validate action parameters
        /// </summary>
        public Schema Schema { get; set; }

        /// <summary>
        /// Action to be executed
        /// </summary>
        public Func<HttpRequest, Task<IActionResult>> Action { get; set; }
    }
}