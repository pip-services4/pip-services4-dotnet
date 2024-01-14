﻿using Microsoft.ApplicationInsights;
using Microsoft.ApplicationInsights.Extensibility;
using PipServices4.Components.Config;
using PipServices4.Components.Context;
using PipServices4.Config.Auth;
using PipServices4.Observability.Count;
using System.Collections.Generic;

namespace PipServices4.Azure.Count
{
    public class AppInsightsCounters : CachedCounters
    {
        private CredentialResolver _credentialResolver = new CredentialResolver();
        private TelemetryClient _client;

        public AppInsightsCounters() { }

        public override void Configure(ConfigParams config)
        {
            base.Configure(config);
            _credentialResolver.Configure(config, true);
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

        protected override void Save(IEnumerable<Counter> counters)
        {
            if (_client == null) Open();

            foreach (var counter in counters)
            {
                switch (counter.Type)
                {
                    case CounterType.Increment:
                        _client.TrackMetric(counter.Name, counter.Count.Value);
                        break;
                    case CounterType.Interval:
                        _client.TrackMetric(counter.Name, counter.Average.Value);
                        break;
                    case CounterType.LastValue:
                        _client.TrackMetric(counter.Name, counter.Last.Value);
                        break;
                    case CounterType.Statistics:
                        _client.TrackMetric(counter.Name, counter.Average.Value);
                        break;
                }
            }

            _client.Flush();
        }
    }
}
