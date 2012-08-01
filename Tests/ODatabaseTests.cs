using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Eastern;

namespace Tests
{
    [TestClass]
    public class ODatabaseTests
    {
        private const string _hostname = "127.0.0.1";
        private const int _port = 2424;
        private const string _databaseName = "test1";
        private const string _username = "admin";
        private const string _password = "admin";

        [TestMethod]
        public void TestDatabaseOpen()
        {
            using (ODatabase database = new ODatabase(_hostname, _port, _databaseName, ODatabaseType.Document, _username, _password))
            {
                Assert.IsTrue(database.SessionID > 0);
                Assert.IsTrue(database.ClustersCount > 0);
                Assert.IsTrue(database.Clusters.Count > 0);
                
                foreach (OCluster cluster in database.Clusters)
                {
                    Assert.IsNotNull(cluster.Name);
                    Assert.IsNotNull(cluster.Type);
                    Assert.IsTrue(cluster.ID >= 0);
                }
            }
        }

        [TestMethod]
        public void TestDatabaseCloseConnection()
        {
            using (ODatabase database = new ODatabase(_hostname, _port, _databaseName, ODatabaseType.Document, _username, _password))
            {
                database.Close();

                Assert.IsTrue(database.SessionID == -1);
            }
        }
    }
}
