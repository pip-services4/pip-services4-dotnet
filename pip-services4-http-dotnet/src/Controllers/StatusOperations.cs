using Microsoft.AspNetCore.Http;
using PipServices4.Commons.Convert;
using PipServices4.Components.Context;
using PipServices4.Components.Refer;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Globalization;
using System.Security.Claims;
using System.Threading.Tasks;

namespace PipServices4.Http.Controllers
{
    public class StatusOperations: RestOperations
    {
        private ContextInfo _contextInfo;
        private IReferences _references2;

        public new void SetReferences(IReferences references)
        {
            base.SetReferences(references);
            _references2 = references;

            _contextInfo = references.GetOneOptional<ContextInfo>(
                new Descriptor("pip-services", "context-info", "*", "*", "*"));
        }

        public Func<HttpRequest, HttpResponse, ClaimsPrincipal, Task> GetStatusOperation()
        {
            return async (req, res, user) => { await StatusAsync(req, res, user); };
        }

        public async Task StatusAsync(HttpRequest request, HttpResponse response, ClaimsPrincipal user)
        {
            dynamic status = new ExpandoObject();
            status.id = _contextInfo?.ContextId;
            status.name = _contextInfo != null ? _contextInfo.Name : "unknown";
            status.description = _contextInfo?.Description;
            status.properties = _contextInfo?.Properties;
            status.uptime = _contextInfo?.Uptime;
            status.start_time = _contextInfo?.StartTime;
            status.current_time = new DateTime().ToUniversalTime().ToString(CultureInfo.InvariantCulture);
            
            var components = new List<string>();
            if (_references2 != null) {
                foreach (var locator in _references2.GetAllLocators())
                {
                    components.Add(locator.ToString());
                }
            }

            status.components = components;
            await SendResultAsync(response, JsonConverter.ToJson(status));
        }
    }
}