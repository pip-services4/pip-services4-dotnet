namespace PipServices4.Data.test.Validate
{
    public class TestSubObject
    {
        public TestSubObject(string id)
        {
            Id = id;
        }

        public string Id { get; set; }
        public float FloatField = 432;
        public object NullProperty { get; set; } = null;
    }
}
