using PipServices4.Expressions.Variants;
using System;
using Xunit;

namespace PipServices4.Expressions.Test.Variants
{
    public class VariantTest
    {
        [Fact]
        public void TestVariants()
        {
            Variant a = new Variant((int)123);
            Assert.Equal(VariantType.Integer, a.Type);
            Assert.Equal(123, a.AsInteger);
            Assert.Equal(123, a.AsObject);

            Variant b = new Variant("xyz");
            Assert.Equal(VariantType.String, b.Type);
            Assert.Equal("xyz", b.AsString);
            Assert.Equal("xyz", b.AsObject);

            Variant c = new Variant(0);
            Assert.Equal(VariantType.Integer, c.Type);
            Assert.Equal(0, c.AsInteger);
            Assert.Equal(0, c.AsObject);

            Variant d = new Variant(null);
            Assert.Equal(VariantType.Null, d.Type);
            Assert.Null(d.AsObject);
        }
    }
}
