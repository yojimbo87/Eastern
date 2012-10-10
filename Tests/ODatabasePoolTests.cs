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

        public ODatabasePoolTests()
        {
            _connection = new OServer(_hostname, _port, _rootName, _rootPassword);

            // create test database
            if (!_connection.DatabaseExist(_databaseName))
            {
                _connection.CreateDatabase(_databaseName, ODatabaseType.Document, OStorageType.Local);
            }
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
            if (_connection.DatabaseExist(_databaseName))
            {
                _connection.DeleteDatabase(_databaseName);
            }

            _connection.Close();
        }
    }
}
