using Benchmark.Data;
using PipBenchmark;
using PipServices4.Components.Config;
using PipServices4.Components.Context;
using PipServices4.Data.Random;
using PipServices4.Redis.Cache;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Benchmark.Benchmark
{
    public class RedisBenchmarkSuite : BenchmarkSuite, IDisposable
    {
        private List<DummyCacheObject> listCacheObjects = new List<DummyCacheObject>();
        private List<string> _ids = new List<string>();
        
        private IContext _context = PipServices4.Components.Context.Context.FromTraceId("Redis.Benchmark");

        private RedisCache _cache = new RedisCache();

        private static readonly ConfigParams CacheConfig = ConfigParams.FromTuples(
            "connection.host", Environment.GetEnvironmentVariable("CACHE_HOST"),
            "connection.port", Environment.GetEnvironmentVariable("CACHE_PORT")
        );

        public RedisBenchmarkSuite() : base("Redis.Benchmark", "Measures performance of Redis Benchmark")
        {
            InitBenchmark();

            //CreateBenchmark("Create", "Measures performance of Add", BenchmarkCreate);
            //CreateBenchmark("Delete", "Measures performance of Delete", BenchmarkDelete);
            CreateBenchmark("Get", "Measures performance of Get", BenchmarkGet);
        }

        private void InitBenchmark()
        {
            _cache.Configure(CacheConfig);
            _cache.OpenAsync(null).Wait();

            for (int i = 0; i < 300000; i++)
            {
                listCacheObjects.Add(RandomDummyCacheObject.GenerateCacheObject());    
                _ids.Add(listCacheObjects[i].Id);
                _cache.StoreAsync(_context, listCacheObjects[i].Id, listCacheObjects[i], 600000).Wait();
            }
        }


        public void Dispose() { }

        public override async void SetUp()
        {
            await Task.Delay(0);
        }


        public void BenchmarkCreate()
        {
            var _object = RandomDummyCacheObject.GenerateCacheObject();

            var result = _cache.StoreAsync(_context, _object.Id, _object, 600000).Result;
            _ids.Add(result.Id);
        }

        public void BenchmarkDelete()
        {
            var id = RandomArray.Pick(_ids.ToArray());

            if (!string.IsNullOrWhiteSpace(id))
            {
                _cache.RemoveAsync(_context, id).Wait();
            }
        }
        public void BenchmarkGet()
        {
            var id = RandomArray.Pick(_ids.ToArray());

            if (!string.IsNullOrWhiteSpace(id))
            {
                var a = _cache.RetrieveAsync<DummyCacheObject>(_context, id).Result;

                if (a == null)
                {
                    var obj = listCacheObjects.FirstOrDefault(t => t.Id == id);
                    _cache.StoreAsync(_context, obj.Id, obj, 600000).Wait();
                }
            }
        }

    }
}
