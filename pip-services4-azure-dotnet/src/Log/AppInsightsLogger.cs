using Microsoft.ApplicationInsights;
using Microsoft.ApplicationInsights.DataContracts;
using Microsoft.ApplicationInsights.Extensibility;
using PipServices4.Components.Config;
using PipServices4.Components.Context;
using PipServices4.Config.Auth;
using PipServices4.Observability.Log;
using System;
using System.Collections.Generic;

namespace PipServices4.Azure.Log
{
    /// <summary>
    /// Class AppInsightsLogger.
    /// </summary>
    /// <seealso cref="PipServices4.Commons.Log.Logger" />
    /// <seealso cref="PipServices4.Commons.Refer.IDescriptable" />
    public class AppInsightsLogger : Logger
    {
        private CredentialResolver _credentialResolver = new CredentialResolver();
        private TelemetryClient _client;

        public override void Configure(ConfigParams config)
        {
            base.Configure(config);
            _credentialResolver.Configure(config, true);
        }

        private SeverityLevel LevelToSeverity(LogLevel level)
        {
            switch (level)
            {
                case LogLevel.Fatal:
                    return SeverityLevel.Critical;
                case LogLevel.Error:
                    return SeverityLevel.Error;
                case LogLevel.Warn:
                    return SeverityLevel.Warning;
                case LogLevel.Info:
                    return SeverityLevel.Information;
                case LogLevel.Debug:
                    return SeverityLevel.Verbose;
                case LogLevel.Trace:
                    return SeverityLevel.Verbose;
            }

            return SeverityLevel.Verbose;
        }

        private void Open()
        {
            var credential = _credentialResolver.LookupAsync(Context.FromTraceId("count")).Result;

            var key = credential.AccessKey
                ?? credential.GetAsNullableString("instrumentation_key")
                ?? credential.GetAsNullableString("InstrumentationKey");

            var config = TelemetryConfiguration.CreateDefault();

            if (key != null)
                config.InstrumentationKey = key;

            _client = new TelemetryClient(config);
        }

        protected override void Write(LogLevel level, IContext context, Exception error, string message)
        {
            if (_client == null)
            {
                Open();
            }

            if (Level < level)
            {
                return;
            }

            if (context != null)
            {
                if (error != null)
                {
                    _client.TrackException(error, new Dictionary<string, string>
                    {
                        { "CorrelationId", context != null ? ContextResolver.GetTraceId(context) : null },
                        { "message", message }
                    });
                }
                else
                {
                    _client.TrackTrace(message, LevelToSeverity(level), new Dictionary<string, string>
                    {
                        { "CorrelationId", context != null ? ContextResolver.GetTraceId(context) : null }
                    });
                }
            }
            else
            {
                if (error != null)
                {
                    _client.TrackException(error);
                }
                else
                {
                    _client.TrackTrace(message, LevelToSeverity(level));
                }
            }
        }

        public void Dump()
        {
            _client.Flush();
        }
    }
}
