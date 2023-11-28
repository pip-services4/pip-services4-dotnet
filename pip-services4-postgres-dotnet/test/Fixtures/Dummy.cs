using PipServices4.Data.Data;
using System;
using System.Runtime.Serialization;

namespace PipServices4.Postgres.Test.Fixtures
{
    [DataContract]
    public class Dummy : IStringIdentifiable
    {
        [DataMember(Name = "id")]
        public string Id { get; set; }

        [DataMember(Name = "key")]
        public string Key { get; set; }

        [DataMember(Name = "content")]
        public string Content { get; set; }

        [DataMember(Name = "create_time_utc")]
        public DateTime CreateTimeUtc { get; set; }

        [DataMember(Name = "sub_dummy")]
        public SubDummy SubDummy { get; set; } = new SubDummy();
    }
}
