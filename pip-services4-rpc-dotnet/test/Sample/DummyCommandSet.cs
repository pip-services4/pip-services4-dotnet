﻿using PipServices4.Components.Exec;
using PipServices4.Data.Query;
using PipServices4.Data.Validate;
using PipServices4.Rpc.Commands;
using System;
using System.Threading.Tasks;

namespace PipServices4.Rpc.Test.Sample
{
    public class DummyCommandSet : CommandSet
    {
        private IDummyService _controller;

        public DummyCommandSet(IDummyService controller)
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
            AddCommand(MakeCheckCorrelationIdCommand());

            // V2
            AddCommand(MakePingCommand());
        }

        private ICommand MakeGetPageByFilterCommand()
        {
            return new Command(
                "get_dummies",
                new ObjectSchema()
                    .WithOptionalProperty("correlation_id", typeof(string))
                    .WithOptionalProperty("filter", new FilterParamsSchema())
                    .WithOptionalProperty("paging", new PagingParamsSchema()),
                async (context, args) => 
                {
                    var filter = FilterParams.FromValue(args.Get("filter"));
                    var paging = PagingParams.FromValue(args.Get("paging"));

                    return await _controller.GetPageByFilterAsync(context, filter, paging);    
                });
        }

        private ICommand MakeGetOneByIdCommand()
        {
            return new Command(
                "get_dummy_by_id",
                new ObjectSchema()
                    .WithRequiredProperty("dummy_id", Commons.Convert.TypeCode.String),
                async (context, args) => 
                {
                    var dummyId = args.GetAsString("dummy_id");
                    return await _controller.GetOneByIdAsync(context, dummyId);                    
                });
        }

        private ICommand MakeCreateCommand()
        {
            return new Command(
                "create_dummy",
                new ObjectSchema()
                    .WithRequiredProperty("dummy", new DummySchema()),
                async (context, args) => 
                {
                    var dummy = ExtractDummy(args);
                    return await _controller.CreateAsync(context, dummy);
                });
        }

        private ICommand MakeUpdateCommand()
        {
            return new Command(
                "update_dummy",
                new ObjectSchema()
                    .WithRequiredProperty("dummy", new DummySchema()),
                async (context, args) =>
                {
                    var dummy = ExtractDummy(args);
                    return await _controller.UpdateAsync(context, dummy);
                });
        }

        private ICommand MakeDeleteByIdCommand()
        {
            return new Command(
                "delete_dummy",
                new ObjectSchema()
                    .WithRequiredProperty("dummy_id", Commons.Convert.TypeCode.String),
                async (context, args) => 
                {
                    var dummyId = args.GetAsString("dummy_id");

                    return await _controller.DeleteByIdAsync(context, dummyId);
                });
        }

        private ICommand MakeCreateWithoutValidationCommand()
        {
            return new Command(
                "create_dummy_without_validation",
                null,
                async (context, parameters) => 
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
                (context, parameters) =>
                {
                    throw new Exception("Dummy error in commandset!");
                });
        }

        private ICommand MakeRaiseControllerExceptionCommand()
        {
            return new Command(
                "raise_exception",
                new ObjectSchema(),
                async (context, parameters) =>
                {
                    await _controller.RaiseExceptionAsync(context);
                    return null;
                });
        }

        private ICommand MakePingCommand()
        {
            return new Command(
                "ping_dummy",
                null,
                async (context, parameters) =>
                {
                    return await _controller.PingAsync();
                });
        }

        private ICommand MakeCheckCorrelationIdCommand()
        {
            return new Command(
                "check_correlation_id",
                new ObjectSchema(),
                async (context, parameters) =>
                {
                    return await _controller.CheckCorrelationId(context);
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