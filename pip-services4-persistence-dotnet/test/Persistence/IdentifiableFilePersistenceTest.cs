﻿using PipServices4.Components.Config;
using PipServices4.Persistence.Persistence;
using System;
using Xunit;

namespace PipServices4.Persistence.Test.Persistence
{
    public sealed class IdentifiableFilePersistenceTest : IDisposable
    {
        private static IdentifiableFilePersistence<PersistenceFixture.Dummy, string> Db 
            = new IdentifiableFilePersistence<PersistenceFixture.Dummy,string>(new JsonFilePersister<PersistenceFixture.Dummy>());
        private static PersistenceFixture Fixture { get; set; }

        private PersistenceFixture GetFixture()
        {
            return new PersistenceFixture(Db, Db, Db, Db, Db, Db, Db, Db);
        }

        public IdentifiableFilePersistenceTest()
        {
            if (Db == null)
                return;

            Db.Configure(ConfigParams.FromTuples("path", nameof(IdentifiableFilePersistenceTest)));

            var task = Db.OpenAsync(null);
            task.Wait();

            task = Db.ClearAsync(null);
            task.Wait();

            Fixture = GetFixture();
        }

        [Fact]
        public void TestCrudOperations()
        {
            if (Fixture == null) return;

            var task = Fixture.TestCrudOperationsAsync();
            task.Wait();
        }

        [Fact]
        public void TestMultithreading()
        {
            if (Fixture == null) return;

            var task = Fixture.TestMultithreading();
            task.Wait();
        }

        public void Dispose()
        {
            var task = Db.CloseAsync(null);
            task.Wait();
        }
    }
}
