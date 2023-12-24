using PipServices4.Expressions.Variants;
using System;
using Xunit;

namespace PipServices4.Expressions.Test.Variants
{
    public class TypeSafeVariantOperationsTest
    {
        [Fact]
        public void TestOperations()
        {
            Variant a = new Variant((int)123);
            IVariantOperations manager = new TypeSafeVariantOperations();

            Variant b = manager.Convert(a, VariantType.Float);
            Assert.Equal(VariantType.Float, b.Type);
            Assert.Equal(123.0, b.AsFloat);

            Variant c = new Variant(2);
            Assert.Equal(125, manager.Add(a, c).AsInteger);
            Assert.Equal(121, manager.Sub(a, c).AsInteger);
            Assert.False(manager.Equal(a, c).AsBoolean);

            Variant[] array = new Variant[] { new Variant("aaa"), new Variant("bbb"), new Variant("ccc"), new Variant("ddd") };
            Variant d = new Variant(array);
            Assert.True(manager.In(d, new Variant("ccc")).AsBoolean);
            Assert.False(manager.In(d, new Variant("eee")).AsBoolean);
            Assert.Equal("bbb", manager.GetElement(d, new Variant(1)).AsString);
        }
    }
}
