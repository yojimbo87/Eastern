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
    public class ODatabasePoolTests
    {
        private const string _hostname = "127.0.0.1";
        private const int _port = 2424;
        private const string _username = "admin";
        private const string _password = "admin";
        private const string _databaseName = "test1";
        private const int _poolSize = 5;

        [TestMethod]
        public void TestCreateDatabasePool()
        {
            EasternClient.CreateDatabasePool(_hostname, _port, _databaseName, ODatabaseType.Document, _username, _password, _poolSize);

            ODatabasePool pool = EasternClient.GetDatabasePool(_hostname, _port, _databaseName, ODatabaseType.Document, _username);
            //int poolSize = EasternClient.GetDatabasePoolSize(_hostname, _port, _databaseName, ODatabaseType.Document, _username);

            Assert.IsTrue(pool.PoolSize == _poolSize);
        }

        [TestMethod]
        public void TestGetDatabase()
        {
            EasternClient.CreateDatabasePool(_hostname, _port, _databaseName, ODatabaseType.Document, _username, _password, _poolSize);

            ODatabase database = EasternClient.GetDatabase(_hostname, _port, _databaseName, ODatabaseType.Document, _username, _password);
            int sessionID = database.SessionID;
            
            Assert.IsTrue(sessionID > 0);
            Assert.IsTrue(database.Size > 0);

            ODatabasePool pool = EasternClient.GetDatabasePool(_hostname, _port, _databaseName, ODatabaseType.Document, _username);

            Assert.IsTrue(pool.CurrentPoolSize == (pool.PoolSize - 1));

            database.Close();

            Assert.IsTrue(pool.CurrentPoolSize == pool.PoolSize);
        }
    }
}
