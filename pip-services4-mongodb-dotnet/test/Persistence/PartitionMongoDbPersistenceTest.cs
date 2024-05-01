using PipServices4.Components.Config;
using PipServices4.Mongodb.Test.Fixtures;
using System;
using System.Threading.Tasks;
using Xunit;

namespace PipServices4.Mongodb.Test.Persistence
{
    [Collection("Sequential")]
    public sealed class PartitionMongoDbPersistenceTest : IDisposable
    {
        private static PartitionMongoDbDummyPersistence Db { get; } = new PartitionMongoDbDummyPersistence();
        private static PersistenceFixture Fixture { get; set; }

        public PartitionMongoDbPersistenceTest()
        {
            var mongoUri = Environment.GetEnvironmentVariable("MONGO_URI");
            var mongoHost = Environment.GetEnvironmentVariable("MONGO_HOST") ?? "localhost";
            var mongoPort = Environment.GetEnvironmentVariable("MONGO_PORT") ?? "27017";
            var mongoDatabase = Environment.GetEnvironmentVariable("MONGO_DB") ?? "test";

            if (mongoUri == null && mongoHost == null)
                return;

            if (Db == null) return;

            Db.Configure(ConfigParams.FromTuples(
                "connection.uri", mongoUri,
                "connection.host", mongoHost,
                "connection.port", mongoPort,
                "connection.database", mongoDatabase
            ));

            Db.OpenAsync(null).Wait();
            Db.ClearAsync(null).Wait();

            Fixture = new PersistenceFixture(Db);
        }

        [Fact]
        public async Task TestCrudOperations()
        {
            await Fixture?.TestCrudOperationsAsync();
        }

        //[Fact]
        //public async Task TestMultithreading()
        //{
        //    await Fixture?.TestMultithreading();
        //}

        [Fact]
        public async Task It_Should_Not_Get_By_Wrong_Id_And_Projection()
        {
            await Fixture?.TestGetByWrongIdAndProjection();
        }

        [Fact]
        public async Task It_Should_Get_By_Id_And_Projection()
        {
            await Fixture?.TestGetByIdAndProjection();
        }

        [Fact]
        public async Task It_Should_Get_By_Id_And_Projection_From_Array()
        {
            await Fixture?.TestGetByIdAndProjectionFromArray();
        }

        [Fact]
        public async Task It_Should_Get_By_Id_And_Wrong_Projection()
        {
            await Fixture?.TestGetByIdAndWrongProjection();
        }

        [Fact]
        public async Task It_Should_Get_By_Id_And_Null_Projection()
        {
            await Fixture?.TestGetByIdAndNullProjection();
        }

        [Fact]
        public async Task It_Should_Get_By_Id_And_Id_Projection()
        {
            await Fixture?.TestGetByIdAndIdProjection();
        }

        [Fact]
        public async Task It_Should_Get_Page_By_Filter()
        {
            await Fixture?.TestGetPageByFilter();
        }

        [Fact]
        public async Task It_Should_Get_Page_By_Projection()
        {
            await Fixture?.TestGetPageByProjection();
        }

        [Fact]
        public async Task It_Should_Get_Page_By_Null_Projection()
        {
            await Fixture?.TestGetPageByNullProjection();
        }

        [Fact]
        public async Task It_Should_Not_Get_Page_By_Wrong_Projection()
        {
            await Fixture?.TestGetPageByWrongProjection();
        }

        [Fact]
        public async Task It_Should_Modify_Object_With_Existing_Properties_By_Selected_Fields()
        {
            await Fixture?.TestModifyExistingPropertiesBySelectedFields();
        }

        [Fact]
        public async Task It_Should_Modify_Object_With_Existing_Properties_By_Selected_Not_Changed_Fields()
        {
            await Fixture?.TestModifyExistingPropertiesBySelectedNotChangedFields();
        }

        [Fact]
        public async Task It_Should_Modify_Object_With_Null_Properties_By_Selected_Fields()
        {
            await Fixture?.TestModifyExistingPropertiesBySelectedFields();
        }

        [Fact]
        public async Task It_Should_Modify_Nested_Collection_By_Selected_Fields()
        {
            await Fixture?.TestModifyNestedCollectionBySelectedFields();
        }

        [Fact]
        public async Task It_Should_Search_Within_Nested_Collection_Filter()
        {
            await Fixture?.TestSearchWithinNestedCollectionByFilter();
        }

        [Fact]
        public async Task It_Should_Search_Within_Nested_Collection_Filter_By_Null_Projection()
        {
            await Fixture?.TestSearchWithinNestedCollectionByFilterAndNullProjection();
        }

        [Fact]
        public async Task It_Should_Search_Within_Deep_Nested_Collection_Filter()
        {
            await Fixture?.TestSearchWithinDeepNestedCollectionByFilter();
        }

        [Fact]
        public async Task It_Should_Search_Within_Deep_Nested_Collection_Filter_By_Null_Projection()
        {
            await Fixture?.TestSearchWithinDeepNestedCollectionByFilterAndNullProjection();
        }

        [Fact]
        public async Task It_Should_Modify_Nested_Collection()
        {
            await Fixture?.TestModifyNestedCollection();
        }

        [Fact]
        public async Task It_Should_Get_Page_By_Ids_Filter()
        {
            await Fixture?.TestGetPageByIdsFilter();
        }

        [Fact]
        public async Task It_Should_Get_Page_By_Array_Of_Keys_Filter()
        {
            await Fixture?.TestGetPageByArrayOfKeysFilter();
        }

        [Fact]
        public async Task It_Should_Get_Page_Sorted_By_One_Field()
        {
            await Fixture?.TestGetPageSortedByOneField();
        }

        [Fact]
        public async Task It_Should_Get_Sales_Orders_Sorted_By_Multiple_Fields()
        {
            await Fixture?.TestGetPageSortedByMultipleFields();
        }

        [Fact]
        public async Task It_Should_Get_Sales_Orders_By_Projection_And_Sorted_By_One_Field()
        {
            await Fixture?.TestGetPageByProjectionAndSortedByOneField();
        }

        [Fact]
        public async Task It_Should_Get_Sales_Orders_By_Projection_And_Sorted_By_Multiple_Fields()
        {
            await Fixture?.TestGetPageByProjectionAndSortedByMultipleFields();
        }

        public void Dispose()
        {
            Db?.CloseAsync(null).Wait();
        }
    }
}
