using PipServices4.Commons.Errors;
using PipServices4.Components.Context;
using PipServices4.Components.Exec;
using PipServices4.Rpc.Commands;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

namespace PipServices4.Rpc.Test.Commands
{
    //[TestClass]
    public sealed class CommandTest
    {
        private class CommandExec : IExecutable
        {
            public Task<object> ExecuteAsync(IContext context, Parameters args)
            {
                string traceId = context != null ? ContextResolver.GetTraceId(context) : null;
                if (traceId == "wrongId")
                    throw new ApplicationException(null, null, null, "Test error");

                return Task.FromResult((object)0);
            }
        }

        [Fact]
        public void TestGetName()
        {
            var command = new Command("name", null, new CommandExec().ExecuteAsync);
            Assert.Equal("name", command.Name);
        }

        [Fact]
        public void testExecute()
        {
            var command = new Command("name", null, new CommandExec().ExecuteAsync);

            Dictionary<int, object> map = new Dictionary<int, object>
            {
                { 8, "title 8" },
                { 11, "title 11" }
            };
            Parameters param = new Parameters(map);

            Assert.Equal(0, command.ExecuteAsync(Context.FromTraceId("a"), param).Result);

            //try
            //{
            //    var result = command.ExecuteAsync("wrongId", param).Result;
            //}
            //catch (ApplicationException e)
            //{
            //    Assert.Equal("Test error", e.Message);
            //}
        }

    }
}
