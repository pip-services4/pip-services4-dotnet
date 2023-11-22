//using System;
//using System.Runtime.Serialization;
//using PipServices3.Commons.Data;
//using PipServices3.MySql.Fixtures;

//namespace PipServices4.MySql.Test.Fixtures
//{
//    [DataContract]
//    public class Dummy2 : IIdentifiable<Integer>
//    {
//        [DataMember(Name = "id")]
//        public Integer Id { get; set; }

//        [DataMember(Name = "key")]
//        public string Key { get; set; }

//        [DataMember(Name = "content")]
//        public string Content { get; set; }

//        [DataMember(Name = "create_time_utc")]
//        public DateTime CreateTimeUtc { get; set; }

//        [DataMember(Name = "sub_dummy")]
//        public SubDummy SubDummy { get; set; } = new SubDummy();
//    }
//}
