using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using System.Diagnostics;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Eastern;

namespace Tests
{
    [TestClass]
    public class ODatabasePoolTests : IDisposable
    {
        private OServer _connection;

        private const string _hostname = "127.0.0.1";
        private const int _port = 2424;
        private const string _rootName = "root";
        private const string _rootPassword = "9F696830A58E8187F6CC36674C666AE73E202DE3B0216C5B8BF8403C276CEE52";
        private const string _databaseName = "tempEasternUniqueTestDatabase01x";
        private const string _username = "admin";
        private const string _password = "admin";
        private const int _poolSize = 5;

        public ODatabasePoolTests()
        {
            _connection = new OServer(_hostname, _port, _rootName, _rootPassword);

            // create test database
            _connection.CreateDatabase(_databaseName, ODatabaseType.Document, OStorageType.Local);
        }

        [TestMethod]
        public void TestCreateDatabasePool()
        {
            EasternClient.CreateDatabasePool(_hostname, _port, _databaseName, ODatabaseType.Document, _username, _password, _poolSize);

            ODatabasePool pool = EasternClient.GetDatabasePool(_hostname, _port, _databaseName, ODatabaseType.Document, _username);
            //int poolSize = EasternClient.GetDatabasePoolSize(_hostname, _port, _databaseName, ODatabaseType.Document, _username);

            Assert.IsTrue(pool.PoolSize == _poolSize);
        }

        /*[TestMethod]
        public void TestGetDatabase()
        {
            EasternClient.CreateDatabasePool(_hostname, _port, _databaseName, ODatabaseType.Document, _username, _password, _poolSize);

            ODatabase database = EasternClient.GetDatabase(_hostname, _port, _databaseName, ODatabaseType.Document, _username, _password);
            int sessionID = database.SessionID;
            
            Assert.IsTrue(sessionID > 0);
            Assert.IsTrue(database.Size > 0);

            ODatabasePool pool = EasternClient.GetDatabasePool(_hostname, _port, _databaseName, ODatabaseType.Document, _username);

            Assert.IsTrue(pool.CurrentPoolSize == (pool.PoolSize - 1));
            Assert.IsFalse(pool.Databases.Contains(database));

            database.Close();

            Assert.IsTrue(pool.CurrentPoolSize == pool.PoolSize);
            Assert.IsTrue(pool.Databases.Contains(database));
        }*/

        public void Dispose()
        {
            // delete test database
            _connection.DeleteDatabase(_databaseName);

            _connection.Close();
        }
    }
}
