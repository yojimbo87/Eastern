using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using System.Configuration;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Eastern;

namespace Tests
{
    [TestClass]
    public class OServerTests
    {
        private string _hostname = ConfigurationManager.AppSettings["hostname"];
        private int _port = int.Parse(ConfigurationManager.AppSettings["port"]);
        private string _rootName = ConfigurationManager.AppSettings["root.name"];
        private string _rootPassword = ConfigurationManager.AppSettings["root.password"];
        private string _databaseName = ConfigurationManager.AppSettings["database.name"];
        private string _username = ConfigurationManager.AppSettings["user.name"];
        private string _password = ConfigurationManager.AppSettings["user.password"];

        [TestMethod]
        public void TestServerConnect()
        {
            using (OServer connection = new OServer(_hostname, _port, _rootName, _rootPassword))
            {
                Assert.IsTrue(connection.SessionID > 0);
            }
        }

        [TestMethod]
        public void TestServerCloseConnection()
        {
            using (OServer connection = new OServer(_hostname, _port, _rootName, _rootPassword))
            {
                connection.Close();

                Assert.IsTrue(connection.SessionID == -1);
            }
        }

        [TestMethod]
        public void TestServerDatabaseExist()
        {
            using (OServer connection = new OServer(_hostname, _port, _rootName, _rootPassword))
            {
                connection.CreateDatabase(_databaseName, ODatabaseType.Document, OStorageType.Local);

                Assert.IsTrue(connection.DatabaseExist(_databaseName));

                connection.DeleteDatabase(_databaseName);
            }
        }

        [TestMethod]
        public void TestServerDatabaseNotExist()
        {
            using (OServer connection = new OServer(_hostname, _port, _rootName, _rootPassword))
            {
                Assert.IsFalse(connection.DatabaseExist("whateverThisShouldNotExist001x"));
            }
        }

        [TestMethod]
        public void TestServerDatabaseCreateDelete()
        {
            using (OServer connection = new OServer(_hostname, _port, _rootName, _rootPassword))
            {
                bool databaseCreateResult = connection.CreateDatabase(_databaseName, ODatabaseType.Document, OStorageType.Local);

                Assert.IsTrue(databaseCreateResult);

                connection.DeleteDatabase(_databaseName);

                Assert.IsFalse(connection.DatabaseExist(_databaseName));
            }
        }

        /*[TestMethod]
        public void TestServerShutdown()
        {
            using (OServer connection = new OServer(_hostname, _port, _rootName, _rootPassword))
            {
                bool shutdownResult = connection.Shutdown();

                Assert.IsTrue(shutdownResult);
            }
        }*/
    }
}
