using PipServices4.Commons.Reflect;
using System;
using Xunit;

namespace PipServices4.Commons.Test.Reflect
{
    //[TestClass]
    public class TypeReflectorTest
    {
        [Fact]
        public void TestGetType()
        {
            Type type = TypeReflector.GetType("PipServices4.Commons.Convert.TypeCode");
            Assert.NotNull(type);

            type = TypeReflector.GetType("PipServices4.Commons.Convert.TypeCode", "PipServices4.Commons");
            Assert.NotNull(type);
        }

        [Fact]
        public void TestCreateInstance()
        {
            //object value = TypeReflector.CreateInstance("PipServices4.Commons.Reflect.TestClass");
            //Assert.NotNull(value);
        }
    }
}
