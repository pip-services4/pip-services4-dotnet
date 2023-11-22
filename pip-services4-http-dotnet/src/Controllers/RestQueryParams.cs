//using System.Collections.Generic;
//using PipServices4.Commons.Convert;
//using PipServices4.Commons.Data;

//namespace PipServices4.Http.Controllers
//{
//    public sealed class RestQueryParams : MultiValueDictionary<string, object>
//    {
//        public RestQueryParams() { }

//        public RestQueryParams(IContext context)
//        {
//            AddCorrelationId(context);
//        }

//        public RestQueryParams(IContext context, FilterParams filter, PagingParams paging)
//        {
//            AddCorrelationId(context);
//            AddFilterParams(filter);
//            AddPagingParams(paging);
//        }

//        public void AddCorrelationId(IContext context)
//        {
//            if (string.IsNullOrWhiteSpace(context))
//                return;

//            Add("correlation_id", context);
//        }

//        public void AddFilterParams(FilterParams filter)
//        {
//            if (filter == null) return;

//            foreach(var entry in filter)
//            {
//                var value = entry.Value;

//                if (value != null)
//                    Add(entry.Key, value);
//            }
//        }

//        public void AddPagingParams(PagingParams paging)
//        {
//            if (paging == null) return;

//            if (paging.Skip != null)
//                Add("skip", StringConverter.ToString(paging.Skip));

//            if (paging.Take != null)
//                Add("take", StringConverter.ToString(paging.Take));

//            Add("total", StringConverter.ToString(paging.Total));
//        }
//    }
//}
