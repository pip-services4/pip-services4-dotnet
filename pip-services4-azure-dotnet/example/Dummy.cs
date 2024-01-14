using Newtonsoft.Json;
using PipServices4.Data.Data;

namespace PipServices4.Azure
{
    public class Dummy : IStringIdentifiable
    {
        public Dummy()
        {
        }

        public Dummy(string id, string key, string content)
        {
            Id = id;
            Key = key;
            Content = content; ;
        }

        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("key")]
        public string Key { get; set; }

        [JsonProperty("content")]
        public string Content { get; set; }
    }
}