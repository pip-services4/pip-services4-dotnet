using PipServices4.Components.Context;
using PipServices4.Observability.Log;
using System;
using System.Collections.Generic;
using Xunit;

namespace PipServices4.Elasticsearch.Test.Fixtures
{
    public sealed class ElasticSearchLoggerFixture
    {
        private readonly TestElasticSearchLogger Logger;

        static readonly List<string> MessageLevelStrings = new List<string> { "FATAL", "ERROR", "WARN", "INFO", "DEBUG", "TRACE" };

        public ElasticSearchLoggerFixture(TestElasticSearchLogger logger)
        {
            Logger = logger;
        }

        public void TestLogLevel()
        {
            Assert.True(Logger.Level >= LogLevel.None);
            Assert.True(Logger.Level <= LogLevel.Trace);
        }

        public void TestSimpleLogging()
        {
            IContext context = Context.FromTraceId("abc123");

            Logger.RemoveAllSavedOutput();
            Logger.Level = LogLevel.Trace;

            Logger.Fatal(context, "Fatal error message");
            Logger.Error(context, "Error message");
            Logger.Warn(context, "Warning message");

            Logger.Dump();

            Logger.Info(context, "Information message");
            Logger.Debug(context, "Debug message");
            Logger.Trace(context, "Trace message");

            Logger.Dump();

            var totalLogCount = 0;
            var batches = 0;
            while (Logger.SavedMessages.TryDequeue(out var messages))
            {
                for(var m = 0; m < messages.Count; ++m)
                {
                    var message = messages[m];
                    
                    Assert.Equal(MessageLevelStrings[totalLogCount + m], message.Level);
                    var firstWord = message.Message.Substring(0, message.Message.IndexOf(' '));

                    if (Enum.TryParse(typeof(LogLevel), firstWord, out var level))
                    {
                        Assert.Equal( message.Level, level.ToString().ToUpper());
                    }

                    var traceId = context != null ? ContextResolver.GetTraceId(context) : null;
                    Assert.Equal(traceId, message.TraceId);
                    if (message.Error != null)
                    {
                        Assert.Equal(traceId, message.Error.TraceId);
                    }
                }

                totalLogCount += messages.Count;
                Assert.True(messages.Count <= 6);
                ++batches;
            }
            Assert.True(batches >= 2);
        }

        public void TestErrorLogging()
        {
            Logger.RemoveAllSavedOutput();
            IContext context = Context.FromTraceId("123abc");
            try
            {
                // Raise an exception
                throw new Exception();
            }
            catch (Exception ex)
            {
                Logger.Fatal(context, ex, "Fatal error");
                Logger.Error(context, ex, "Recoverable error");
            }

            Logger.Dump();

            var batchCount = 1;

            while (Logger.SavedMessages.TryDequeue(out var messages))
            {
                Assert.True(batchCount <= 1);

                Assert.Equal(2, messages.Count);
                
                Assert.Equal("FATAL", messages[0].Level);
                Assert.Equal("ERROR", messages[1].Level);

                var traceId = context != null ? ContextResolver.GetTraceId(context) : null;

                Assert.Equal(traceId, messages[0].TraceId);
                Assert.Equal(traceId, messages[1].TraceId);

                Assert.Equal(traceId, messages[0].Error.TraceId);
                Assert.Equal(traceId, messages[1].Error.TraceId);

                ++batchCount;
            }
        }
    }
}