using PipServices4.Grpc.Protos;
using PipServices4.Components.Context;
using PipServices4.Data.Query;
using PipServices4.Grpc.Clients;
using System;
using System.Linq;
using System.Threading.Tasks;
using ProtoDummy = PipServices4.Grpc.Protos.Dummy;
using PublicDummy = PipServices4.Grpc.Dummy;

namespace PipServices4.Grpc.Clients
{
    public class DummyGrpcClient : GrpcClient, IDummyClient
	{
		public DummyGrpcClient()
			: base("dummies")
		{ 
		}

		public async Task<PublicDummy> CreateAsync(IContext context, PublicDummy entity)
		{
			var request = new DummyObjectRequest
			{
				TraceId = context != null ? ContextResolver.GetTraceId(context) : null,
				Dummy = ConvertFromPublic(entity)
			};

			var item = await CallAsync<DummyObjectRequest, ProtoDummy>("create_dummy", request);

			return ConvertToPublic(item);
		}

		public async Task<PublicDummy> DeleteByIdAsync(IContext context, string id)
		{
			var request = new DummyIdRequest
			{
				TraceId = context != null ? ContextResolver.GetTraceId(context) : null,
				DummyId = id
			};

			var item = await CallAsync<DummyIdRequest, ProtoDummy>("delete_dummy_by_id", request);

			return ConvertToPublic(item);
		}

		public async Task<PublicDummy> GetOneByIdAsync(IContext context, string id)
		{
			var request = new DummyIdRequest
			{
				TraceId = context != null ? ContextResolver.GetTraceId(context) : null,
				DummyId = id
			};

			var item = await CallAsync<DummyIdRequest, ProtoDummy>("get_dummy_by_id", request);

			return ConvertToPublic(item);
		}

		public async Task<DataPage<PublicDummy>> GetPageByFilterAsync(IContext context, FilterParams filter, PipServices4.Data.Query.PagingParams paging)
		{
			var request = new DummiesPageRequest
			{
				TraceId = context != null ? ContextResolver.GetTraceId(context) : null,
				Paging = new Grpc.Protos.PagingParams()
			};
     		request.Filter.Add(filter);
			if (paging.Skip.HasValue) request.Paging.Skip = paging.Skip.Value;
			if (paging.Take.HasValue) request.Paging.Take = Convert.ToInt32(paging.Take.Value);

			var page = await CallAsync<DummiesPageRequest, DummiesPage>("get_dummies", request);

			var result = new DataPage<PublicDummy>
			{
				Data = page.Data.Select(x => ConvertToPublic(x)).ToList(),
				Total = page.Total
			};

			return result;
		}

		public async Task<PublicDummy> UpdateAsync(IContext context, PublicDummy entity)
		{
			var request = new DummyObjectRequest
			{
				TraceId = context != null ? ContextResolver.GetTraceId(context) : null,
				Dummy = ConvertFromPublic(entity)
			};

			var item = await CallAsync<DummyObjectRequest, ProtoDummy>("update_dummy", request);
			return ConvertToPublic(item);
		}

		private static PublicDummy ConvertToPublic(ProtoDummy dummy)
		{
			if (dummy == null || dummy.Id == "") return null;
			return new PublicDummy
			{
				Id = dummy.Id,
				Key = dummy.Key,
				Content = dummy.Content
			};
		}

		private static ProtoDummy ConvertFromPublic(PublicDummy dummy)
		{
			if (dummy == null) return null;
			return new ProtoDummy
			{
				Id = dummy.Id,
				Key = dummy.Key,
				Content = dummy.Content
			};
		}
	}
}
