using PipServices4.Components.Exec;
using PipServices4.Data.Query;
using PipServices4.Data.Validate;
using PipServices4.Rpc.Commands;
using System;
using System.Threading.Tasks;

namespace PipServices4.Http
{
    public class DummyCommandSet : CommandSet
    {
        private IDummyController _controller;

        public DummyCommandSet(IDummyController controller)
        {
            _controller = controller;

            AddCommand(MakeGetPageByFilterCommand());
            AddCommand(MakeGetOneByIdCommand());
            AddCommand(MakeCreateCommand());
            AddCommand(MakeUpdateCommand());
            AddCommand(MakeDeleteByIdCommand());
            // Commands for errors
            AddCommand(MakeCreateWithoutValidationCommand());
            AddCommand(MakeRaiseCommandSetExceptionCommand());
            AddCommand(MakeRaiseControllerExceptionCommand());
            AddCommand(MakeCheckTraceIdCommand());

            // V2
            AddCommand(MakePingCommand());
        }

        private ICommand MakeGetPageByFilterCommand()
        {
            return new Command(
                "get_dummies",
                new ObjectSchema()
                    .WithOptionalProperty("trace_id", typeof(string))
                    .WithOptionalProperty("filter", new FilterParamsSchema())
                    .WithOptionalProperty("paging", new PagingParamsSchema()),
                async (traceId, args) => 
                {
                    var filter = FilterParams.FromValue(args.Get("filter"));
                    var paging = PagingParams.FromValue(args.Get("paging"));

                    return await _controller.GetPageByFilterAsync(traceId, filter, paging);    
                });
        }

        private ICommand MakeGetOneByIdCommand()
        {
            return new Command(
                "get_dummy_by_id",
                new ObjectSchema()
                    .WithRequiredProperty("dummy_id", Commons.Convert.TypeCode.String),
                async (traceId, args) => 
                {
                    var dummyId = args.GetAsString("dummy_id");
                    return await _controller.GetOneByIdAsync(traceId, dummyId);                    
                });
        }

        private ICommand MakeCreateCommand()
        {
            return new Command(
                "create_dummy",
                new ObjectSchema()
                    .WithRequiredProperty("dummy", new DummySchema()),
                async (traceId, args) => 
                {
                    var dummy = ExtractDummy(args);
                    return await _controller.CreateAsync(traceId, dummy);
                });
        }

        private ICommand MakeUpdateCommand()
        {
            return new Command(
                "update_dummy",
                new ObjectSchema()
                    .WithRequiredProperty("dummy", new DummySchema()),
                async (traceId, args) =>
                {
                    var dummy = ExtractDummy(args);
                    return await _controller.UpdateAsync(traceId, dummy);
                });
        }

        private ICommand MakeDeleteByIdCommand()
        {
            return new Command(
                "delete_dummy",
                new ObjectSchema()
                    .WithRequiredProperty("dummy_id", Commons.Convert.TypeCode.String),
                async (traceId, args) => 
                {
                    var dummyId = args.GetAsString("dummy_id");

                    return await _controller.DeleteByIdAsync(traceId, dummyId);
                });
        }

        private ICommand MakeCreateWithoutValidationCommand()
        {
            return new Command(
                "create_dummy_without_validation",
                null,
                async (traceId, parameters) => 
                {
                    await Task.Delay(0);
                    return null;
                });
        }

        private ICommand MakeRaiseCommandSetExceptionCommand()
        {
            return new Command(
                "raise_commandset_error",
                new ObjectSchema()
                    .WithRequiredProperty("dummy", new DummySchema()),
                (traceId, parameters) =>
                {
                    throw new Exception("Dummy error in commandset!");
                });
        }

        private ICommand MakeRaiseControllerExceptionCommand()
        {
            return new Command(
                "raise_exception",
                new ObjectSchema(),
                async (traceId, parameters) =>
                {
                    await _controller.RaiseExceptionAsync(traceId);
                    return null;
                });
        }

        private ICommand MakePingCommand()
        {
            return new Command(
                "ping_dummy",
                null,
                async (traceId, parameters) =>
                {
                    return await _controller.PingAsync();
                });
        }

        private ICommand MakeCheckTraceIdCommand()
        {
            return new Command(
                "check_trace_id",
                new ObjectSchema(),
                async (traceId, parameters) =>
                {
                    return await _controller.CheckTraceId(traceId);
                });
        }

        private static Dummy ExtractDummy(Parameters args)
        {
            var map = args.GetAsMap("dummy");

            var id = map.GetAsStringWithDefault("id", string.Empty);
            var key = map.GetAsStringWithDefault("key", string.Empty);
            var content = map.GetAsStringWithDefault("content", string.Empty);
            var flag = map.GetAsBooleanWithDefault("flag", false);

            var dummy = new Dummy(id, key, content, flag);
            return dummy;
        }

    }
}