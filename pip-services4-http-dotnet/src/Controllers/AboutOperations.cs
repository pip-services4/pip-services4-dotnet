using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Primitives;
using PipServices4.Commons.Convert;
using PipServices4.Components.Context;
using PipServices4.Components.Refer;
using System;
using System.Dynamic;
using System.Globalization;
using System.IO;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace PipServices4.Http.Controllers
{
    public class AboutOperations : RestOperations
    {
        private ContextInfo _contextInfo;

        public new void SetReferences(IReferences references)
        {
            base.SetReferences(references);

            _contextInfo = references.GetOneOptional<ContextInfo>(
                new Descriptor("pip-services", "context-info", "*", "*", "*"));
        }

        public Func<HttpRequest, HttpResponse, ClaimsPrincipal, Task> GetAboutOperation()
        {
            return async (req, res, user) => { await AboutAsync(req, res, user); };
        }

        public async Task AboutAsync(HttpRequest request, HttpResponse response, ClaimsPrincipal user)
        {
            dynamic about = new ExpandoObject();
            about.server = new ExpandoObject();
            about.server.name = _contextInfo != null ? this._contextInfo.Name : "unknown";
            about.server.description = _contextInfo?.Description;
            about.server.properties = _contextInfo?.Properties;
            about.server.uptime = _contextInfo?.Uptime;
            about.server.start_time = _contextInfo?.StartTime;
            about.server.current_time = new DateTime().ToUniversalTime().ToString(CultureInfo.InvariantCulture);
            about.server.protocol = request?.Protocol;
            about.server.host = request?.Host;
            about.server.url = request?.Path;
            about.server.ip =
                request != null && request.Headers != null &&
                request.Headers.TryGetValue("x-forwarded-for", out StringValues ip)
                    ? ip.ToArray()[0]
                    : null;

            // System.PlatformNotSupportedException: This instance contains state that cannot be serialized and deserialized on this platform.
            //about.client = new ExpandoObject();
            //about.client.user = JsonConverter.ToJson(user ?? new object());

            using (Stream ms = response.Body)
            {
                var jsonString = JsonConverter.ToJson((object) about);

                var sw = new StreamWriter(ms, Encoding.Unicode);
                try
                {
                    // Method Seek is not supported.
                    //ms.Seek(0, SeekOrigin.End);

                    await sw.WriteAsync(jsonString);
                }
                finally
                {
                    sw.Dispose();
                }
            }
        }
    }
}