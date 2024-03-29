using Grpc.Core;
using PipServices4.Commons.Convert;
using PipServices4.Components.Context;
using PipServices4.Components.Refer;
using PipServices4.Data.Validate;
using PipServices4.Grpc.Protos;
using System.Linq;
using System.Threading.Tasks;
using ProtoDummy = PipServices4.Grpc.Protos.Dummy;

namespace PipServices4.Grpc.Controllers
{
    public class DummyGrpcController : GrpcController
    {
        private IDummyService _service;

        public DummyGrpcController()
            : base("dummies")
        {
            _dependencyResolver.Put("service", new Descriptor("pip-services4-dummies", "service", "default", "*", "*"));
        }

        public override void SetReferences(IReferences references)
        {
            base.SetReferences(references);

            _service = _dependencyResolver.GetOneRequired<IDummyService>("service");
        }

        public async Task<DummiesPage> GetDummiesAsync(DummiesPageRequest request, ServerCallContext context)
        {
            var traceId = request.TraceId;
            var filter = new Data.Query.FilterParams(request.Filter);
            var paging = new Data.Query.PagingParams(request.Paging.Skip, request.Paging.Take, request.Paging.Total);

            var page = await _service.GetPageByFilterAsync(Context.FromTraceId(traceId), filter, paging);

            var data = new Google.Protobuf.Collections.RepeatedField<ProtoDummy>();

            var response = new DummiesPage { Total = page.Total ?? 0 };
            response.Data.AddRange(page.Data.Select(x => ConvertToPublic(x)));

            return response;
        }

        public async Task<ProtoDummy> GetDummyByIdAsync(DummyIdRequest request, ServerCallContext context)
        {
            var traceId = request.TraceId;
            var item = await _service.GetOneByIdAsync(Context.FromTraceId(traceId), request.DummyId);
            return ConvertToPublic(item);
        }

        public async Task<ProtoDummy> CreateDummyAsync(DummyObjectRequest request, ServerCallContext context)
        {
            var traceId = request.TraceId;
            var item = await _service.CreateAsync(Context.FromTraceId(traceId), ConvertFromPublic(request.Dummy));
            return ConvertToPublic(item);
        }

        public async Task<ProtoDummy> UpdateDummyAsync(DummyObjectRequest request, ServerCallContext context)
        {
            var traceId = request.TraceId;
            var item = await _service.UpdateAsync(Context.FromTraceId(traceId), ConvertFromPublic(request.Dummy));
            return ConvertToPublic(item);
        }

        public async Task<ProtoDummy> DeleteDummyByIdAsync(DummyIdRequest request, ServerCallContext context)
        {
            var traceId = request.TraceId;
            var item = await _service.DeleteByIdAsync(Context.FromTraceId(traceId), request.DummyId);
            return ConvertToPublic(item);
        }

        private static ProtoDummy ConvertToPublic(Dummy dummy)
        {
            if (dummy == null) return null;
            return new ProtoDummy
            {
                Id = dummy.Id,
                Key = dummy.Key,
                Content = dummy.Content
            };
        }

        private static Dummy ConvertFromPublic(ProtoDummy dummy)
        {
            if (dummy == null) return null;
            return new Dummy
            {
                Id = dummy.Id,
                Key = dummy.Key,
                Content = dummy.Content
            };
        }

		protected override object ConvertFromPublic<TRequest>(TRequest request)
		{
            if (request is DummiesPageRequest pageRequest) return new
            {
                filter = new Data.Query.FilterParams(pageRequest.Filter),
                paging = new Data.Query.PagingParams(pageRequest.Paging.Skip, pageRequest.Paging.Take, pageRequest.Paging.Total)
            };

            if (request is DummyIdRequest idRequest) return new 
            { 
                dummy_id = idRequest.DummyId 
            };

            if (request is DummyObjectRequest objectRequest) return new 
            { 
                dummy = ConvertFromPublic(objectRequest.Dummy) 
            };
            
			return request;
		}

		protected override void OnRegister()
        {
            RegisterMethod<DummiesPageRequest, DummiesPage>(
                "get_dummies",
                new ObjectSchema()
                    .WithOptionalProperty("paging", new PagingParamsSchema())
                    .WithOptionalProperty("filter", new FilterParamsSchema()),
                GetDummiesAsync
            );

            RegisterMethod<DummyIdRequest, ProtoDummy>(
                "get_dummy_by_id",
                new ObjectSchema()
                    .WithRequiredProperty("dummy_id", TypeCode.String),
                GetDummyByIdAsync
            );

            RegisterMethod<DummyObjectRequest, ProtoDummy>(
                "create_dummy",
                new ObjectSchema()
                   .WithRequiredProperty("dummy", new DummySchema()),
                CreateDummyAsync
            );

            RegisterMethod<DummyObjectRequest, ProtoDummy>(
                "update_dummy",
                new ObjectSchema()
                   .WithRequiredProperty("dummy", new DummySchema()), 
                UpdateDummyAsync
            );
            
            RegisterMethod<DummyIdRequest, ProtoDummy>(
                "delete_dummy_by_id", 
                new ObjectSchema()
                    .WithRequiredProperty("dummy_id", TypeCode.String),
                DeleteDummyByIdAsync
            );
        }
    }
}