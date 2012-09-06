using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Eastern;

namespace Tests
{
    [TestClass]
    public class OServerTests
    {
        private const string _hostname = "127.0.0.1";
        private const int _port = 2424;
        private const string _rootName = "root";
        private const string _rootPassword = "84079F1F2D9C6DB52DFE94A5F4B0D9F33C2390E70931E16AF77E191B929C857C";
        //private const string _rootPassword = "root";
        private const string _username = "admin";
        private const string _password = "admin";
        private const string _newDatabaseName = "tempEasternUniqueTestDatabase0001x";

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
                connection.CreateDatabase(_newDatabaseName, ODatabaseType.Document, OStorageType.Local);

                Assert.IsTrue(connection.DatabaseExist(_newDatabaseName));

                connection.DeleteDatabase(_newDatabaseName);
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
                bool databaseCreateResult = connection.CreateDatabase(_newDatabaseName, ODatabaseType.Document, OStorageType.Local);

                Assert.IsTrue(databaseCreateResult);

                connection.DeleteDatabase(_newDatabaseName);

                Assert.IsFalse(connection.DatabaseExist(_newDatabaseName));
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
