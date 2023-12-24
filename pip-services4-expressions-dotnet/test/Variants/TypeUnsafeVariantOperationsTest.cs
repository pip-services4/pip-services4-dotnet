using PipServices4.Expressions.Variants;
using System;
using Xunit;

namespace PipServices4.Expressions.Test.Variants
{
    public class TypeUnsafeVariantOperationsTest
    {
        [Fact]
        public void TestOperations()
        {
            Variant a = new Variant("123");
            IVariantOperations manager = new TypeUnsafeVariantOperations();

            Variant b = manager.Convert(a, VariantType.Float);
            Assert.Equal(VariantType.Float, b.Type);
            Assert.Equal(123.0, b.AsFloat);

            Variant c = new Variant(2);
            Assert.Equal(125.0, manager.Add(b, c).AsFloat);
            Assert.Equal(121.0, manager.Sub(b, c).AsFloat);
            Assert.True(manager.Equal(a, b).AsBoolean);
        }
    }
}
