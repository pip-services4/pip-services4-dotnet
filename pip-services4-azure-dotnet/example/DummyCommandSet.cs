using PipServices4.Components.Context;
using PipServices4.Components.Exec;
using PipServices4.Data.Query;
using PipServices4.Data.Validate;
using PipServices4.Rpc.Commands;
using System;

namespace PipServices4.Azure
{
    public class DummyCommandSet : CommandSet
    {
        private IDummyService _controller;

        public DummyCommandSet(IDummyService controller)
        {
            this._controller = controller;

            AddCommand(MakeGetPageByFilterCommand());
            AddCommand(MakeGetOneByIdCommand());
            AddCommand(MakeCreateCommand());
            AddCommand(MakeUpdateCommand());
            AddCommand(MakeDeleteByIdCommand());
        }

        private ICommand MakeGetPageByFilterCommand()
        {
            return new Command(
                "get_dummies",

                new ObjectSchema()
                    .WithOptionalProperty("filter", new FilterParamsSchema())
                    .WithOptionalProperty("paging", new PagingParamsSchema()),
                async (IContext context, Parameters args) =>
                {
                    var filter = FilterParams.FromValue(args.Get("filter"));
                    var paging = PagingParams.FromValue(args.Get("paging"));
                    return await _controller.GetPageByFilterAsync(context, filter, paging);
                }
            );
        }

        private ICommand MakeGetOneByIdCommand()
        {
            return new Command(
                "get_dummy_by_id",

                new ObjectSchema()
                    .WithRequiredProperty("dummy_id", TypeCode.String),
                async (IContext context, Parameters args) =>
                {
                    var id = args.GetAsString("dummy_id");
                    return await _controller.GetOneByIdAsync(context, id);
                }
            );
        }

        private ICommand MakeCreateCommand()
        {
            return new Command(
                "create_dummy",

                new ObjectSchema()
                    .WithRequiredProperty("dummy", new DummySchema()),
                async (IContext context, Parameters args) =>
                {
                    Dummy entity = ExtractDummy(args);
                    return await _controller.CreateAsync(context, entity);
                }
            );
        }

        private ICommand MakeUpdateCommand()
        {
            return new Command(
                "update_dummy",

                new ObjectSchema()
                    .WithRequiredProperty("dummy", new DummySchema()),
                async (IContext context, Parameters args) =>
                {
                    Dummy entity = ExtractDummy(args);
                    return await _controller.UpdateAsync(context, entity);
                }
            );
        }

        private ICommand MakeDeleteByIdCommand()
        {
            return new Command(
                "delete_dummy",

                new ObjectSchema()
                    .WithRequiredProperty("dummy_id", TypeCode.String),
                async (IContext context, Parameters args) =>
                {
                    var id = args.GetAsString("dummy_id");
                    return await _controller.DeleteByIdAsync(context, id);
                }
            );
        }


        private static Dummy ExtractDummy(Parameters args)
        {
            var map = args.GetAsMap("dummy");

            var id = map.GetAsStringWithDefault("id", string.Empty);
            var key = map.GetAsStringWithDefault("key", string.Empty);
            var content = map.GetAsStringWithDefault("content", string.Empty);

            var dummy = new Dummy(id, key, content);
            return dummy;
        }
    }
}