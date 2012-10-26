using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using System.Diagnostics;
using System.Configuration;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Eastern;

namespace Tests
{
    [TestClass]
    public class ODatabasePoolTests : IDisposable
    {
        private OServer _connection;

        private string _hostname = ConfigurationManager.AppSettings["hostname"];
        private int _port = int.Parse(ConfigurationManager.AppSettings["port"]);
        private string _rootName = ConfigurationManager.AppSettings["root.name"];
        private string _rootPassword = ConfigurationManager.AppSettings["root.password"];
        private string _databaseName = ConfigurationManager.AppSettings["database.name"];
        private string _username = ConfigurationManager.AppSettings["user.name"];
        private string _password = ConfigurationManager.AppSettings["user.password"];
        private int _poolSize = 5;
        private string _poolAlias = "myOrientDatabasePool";

        public ODatabasePoolTests()
        {
            _connection = new OServer(_hostname, _port, _rootName, _rootPassword);

            // if database exists, delete it
            if (_connection.DatabaseExist(_databaseName))
            {
                _connection.DeleteDatabase(_databaseName);
            }

            _connection.CreateDatabase(_databaseName, ODatabaseType.Document, OStorageType.Local);

            // create database pool
            EasternClient.CreateDatabasePool(_hostname, _port, _databaseName, ODatabaseType.Document, _username, _password, _poolSize, _poolAlias);
        }

        [TestMethod]
        public void TestDatabasePoolSize()
        {
            DatabasePool pool = EasternClient.GetDatabasePool(_poolAlias);

            Assert.IsTrue(pool.PoolSize == _poolSize);
        }

        [TestMethod]
        public void TestGetDatabase()
        {
            ODatabase database = new ODatabase(_poolAlias);

            Assert.IsTrue(database.SessionID > 0);

            DatabasePool pool = EasternClient.GetDatabasePool(_poolAlias);

            Assert.IsTrue(pool.CurrentPoolSize == (pool.PoolSize - 1));
            Assert.IsFalse(pool.ContainsDatabaseSession(database.SessionID));

            database.Close();

            Assert.IsTrue(pool.CurrentPoolSize == pool.PoolSize);
            Assert.IsTrue(pool.ContainsDatabaseSession(database.SessionID));
        }

        public void Dispose()
        {
            // delete test database
            if (_connection.DatabaseExist(_databaseName))
            {
                _connection.DeleteDatabase(_databaseName);
            }

            _connection.Close();
        }
    }
}
