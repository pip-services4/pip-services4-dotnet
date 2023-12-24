using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using PipServices4.Expressions.Variants;
using Xunit;
using PipServices4.Expressions.Calculator.Functions;

namespace PipServices4.Expressions.Test.Calculator.functions
{
    public class FunctionCollectionTest
    {
        private Task<Variant> TestFunc(
            IList<Variant> parameters, IVariantOperations variantOperations)
        {
            return Task.FromResult(new Variant());
        }

        [Fact]
        public void TestAddRemoveFunctions()
        {
            var collection = new FunctionCollection();

            var func1 = new DelegatedFunction("ABC", TestFunc);
            collection.Add(func1);
            Assert.Equal(1, collection.Length);

            var func2 = new DelegatedFunction("XYZ", TestFunc);
            collection.Add(func2);
            Assert.Equal(2, collection.Length);

            var index = collection.FindIndexByName("abc");
            Assert.Equal(0, index);

            var func = collection.FindByName("Xyz");
            Assert.Equal(func2, func);

            collection.Remove(0);
            Assert.Equal(1, collection.Length);

            collection.RemoveByName("XYZ");
            Assert.Equal(0, collection.Length);
        }
    }
}
