﻿using PipServices4.Commons.Mapper;
using System.Collections.Generic;
using Xunit;


namespace PipServices4.Commons.Test.Mapper
{
    public sealed class ObjectArrayMapperTest
    {
        internal sealed class ClassA
        {
            public int Property1 { get; set; }
            public string Property2 { get; set; }
        }

        internal sealed class ClassB
        {
            public int Property1 { get; set; }
            public string Property2 { get; set; }
            public IEnumerable<object> Property3 { get; set; }
            public ClassB()
            {
                //Target collection must be created 
                Property3 = new List<object>();
            }
        }

        [Fact]
        public void It_Should_Map_Array_Of_Objects()
        {
            var objectA1 = new ClassA()
            {
                Property1 = 101,
                Property2 = "Property2.1",
            };

            var objectA2 = new ClassA()
            {
                Property1 = 102,
                Property2 = "Property2.2",
            };

            var arrayOfObjectsA = new List<ClassA>() { objectA1, objectA2 };
            var arrayOfObjectsB = new List<ClassB>();

            foreach (var objectA in arrayOfObjectsA)
            {
                arrayOfObjectsB.Add(ObjectMapper.MapTo<ClassB>(objectA));
            }

            Assert.Equal(arrayOfObjectsA.Count, arrayOfObjectsB.Count);
        }
    }
}
