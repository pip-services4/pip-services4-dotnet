using PipServices4.Commons.Convert;
using PipServices4.Commons.Errors;
using PipServices4.Components.Config;
using PipServices4.Components.Context;
using PipServices4.Components.Refer;
using PipServices4.Config.Auth;
using PipServices4.Http.Clients;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace PipServices4.Datadog.Clients
{
    public class DataDogLogClient : RestClient
    {
        private ConfigParams _defaultConfig = ConfigParams.FromTuples(
            "connection.protocol", "https",
            "connection.host", "http-intake.logs.datadoghq.com",
            "connection.port", 443,
            "credential.internal_network", "true"
        );

        private CredentialResolver _credentialResolver = new CredentialResolver();

        public DataDogLogClient(ConfigParams config = null)
            : base()
        {
            if (config != null) Configure(config);
            _baseRoute = "v1";
        }

        public override void Configure(ConfigParams config)
        {
            config = _defaultConfig.Override(config);
            base.Configure(config);
            _credentialResolver.Configure(config);
        }

        public override void SetReferences(IReferences references)
        {
            base.SetReferences(references);
            _credentialResolver.SetReferences(references);
        }

        public override async Task OpenAsync(IContext context)
        {
            var credential = await _credentialResolver.LookupAsync(context);

            if (credential == null || credential.AccessKey == null)
            {
                throw new ConfigException(context != null ? ContextResolver.GetTraceId(context) : null, 
                    "NO_ACCESS_KEY", "Missing access key in credentials");
            }

            await base.OpenAsync(context);

            _client.DefaultRequestHeaders.Add("DD-API-KEY", credential.AccessKey);
        }

        public async Task SendLogsAsync(IContext context, IEnumerable<DataDogLogMessage> messages)
        {
            var data = ConvertMessages(messages);

			//Commented instrumentation because otherwise it will never stop sending logs...
            //let timing = this.instrument(context, "datadog.send_logs");
			try
			{
				await ExecuteAsync<object>(context, HttpMethod.Post, "input", data);
			}
			catch (Exception ex)
			{
				//timing.endTiming();
				InstrumentError(context, "datadog.send_logs", ex, true);
			}
		}

        private string ConvertTags(IDictionary<string, string> tags)
        {
            if (tags == null) return null;
            return string.Join(",", tags.Select(x => string.Format($"{x.Key}:{x.Value}")));
        }

        private Dictionary<string, string> ConvertMessage(DataDogLogMessage message)
        {
            var result = new Dictionary<string, string>
            {
                { "timestamp", StringConverter.ToString(message.Time ?? DateTime.UtcNow) },
                { "status", message.Status ?? "INFO" },
                { "ddsource", message.Source ?? "pip-services" },
                // { "source", message.Source ?? "pip-services" },
                { "service", message.Service },
                { "message", message.Message },
            };

            if (message.Tags != null)
                result["ddtags"] = ConvertTags(message.Tags);
            if (message.Host != null)
                result["host"] = message.Host;
            if (message.LoggerName != null)
                result["logger.name"] = message.LoggerName;
            if (message.ThreadName != null)
                result["logger.thread_name"] = message.ThreadName;
            if (message.ErrorMessage != null)
                result["error.message"] = message.ErrorMessage;
            if (message.ErrorKind != null)
                result["error.kind"] = message.ErrorKind;
            if (message.ErrorStack != null)
                result["error.stack"] = message.ErrorStack;

            return result;
        }

        private List<Dictionary<string, string>> ConvertMessages(IEnumerable<DataDogLogMessage> messages)
        {
            return messages.Select(x => ConvertMessage(x)).ToList();
        }
    }
}
