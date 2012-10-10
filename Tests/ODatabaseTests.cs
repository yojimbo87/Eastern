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
    public class ODatabaseTests : IDisposable
    {
        private OServer _connection;

        private string _hostname =  ConfigurationManager.AppSettings["hostname"];
        private int _port = int.Parse(ConfigurationManager.AppSettings["port"]);
        private string _rootName = ConfigurationManager.AppSettings["root.name"];
        private string _rootPassword = ConfigurationManager.AppSettings["root.password"];
        private string _databaseName = ConfigurationManager.AppSettings["database.name"];
        private string _username = ConfigurationManager.AppSettings["user.name"];
        private string _password = ConfigurationManager.AppSettings["user.password"];

        public ODatabaseTests()
        {
            _connection = new OServer(_hostname, _port, _rootName, _rootPassword);

            // create test database
            if (!_connection.DatabaseExist(_databaseName))
            {
                _connection.CreateDatabase(_databaseName, ODatabaseType.Document, OStorageType.Local);
            }
        }

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

        [TestMethod]
        public void TestDatabaseReload()
        {
            using (ODatabase database = new ODatabase(_hostname, _port, _databaseName, ODatabaseType.Document, _username, _password))
            {
                Assert.IsTrue(database.ClustersCount > 0);

                database.Reload();

                Assert.IsTrue(database.ClustersCount > 0);
            }
        }

        [TestMethod]
        public void TestDatabaseSize()
        {
            using (ODatabase database = new ODatabase(_hostname, _port, _databaseName, ODatabaseType.Document, _username, _password))
            {
                Assert.IsTrue(database.Size > 0);
            }
        }

        [TestMethod]
        public void TestDatabaseRecordsCount()
        {
            using (ODatabase database = new ODatabase(_hostname, _port, _databaseName, ODatabaseType.Document, _username, _password))
            {
                Assert.IsTrue(database.RecordsCount > 0);
            }
        }

        [TestMethod]
        public void TestDatabaseClusterAddRemove()
        {
            using (ODatabase database = new ODatabase(_hostname, _port, _databaseName, ODatabaseType.Document, _username, _password))
            {
                OCluster newCluster = database.AddCluster(OClusterType.Physical, "tempClusterTest001x");

                Assert.IsTrue(newCluster.ID >= 0);
                Assert.IsTrue(!string.IsNullOrEmpty(newCluster.Name));
                Assert.IsTrue(newCluster.RecordsCount == 0);

                database.Reload();

                OCluster cluster = database.Clusters.Find(x => x.ID == newCluster.ID);

                Assert.IsTrue(cluster.ID >= newCluster.ID);
                Assert.IsTrue(cluster.Name.Equals(newCluster.Name, StringComparison.CurrentCultureIgnoreCase));
                Assert.IsTrue(cluster.RecordsCount == newCluster.RecordsCount);

                database.RemoveCluster(newCluster.ID);

                OCluster cluster2 = database.Clusters.Find(x => x.ID == newCluster.ID);

                Assert.IsNull(cluster2);

                database.Reload();

                OCluster cluster3 = database.Clusters.Find(x => x.ID == newCluster.ID);

                Assert.IsNull(cluster3);
            }
        }

        [TestMethod]
        public void TestDatabaseClusterRecordsCount()
        {
            using (ODatabase database = new ODatabase(_hostname, _port, _databaseName, ODatabaseType.Document, _username, _password))
            {
                foreach (OCluster cluster in database.Clusters)
                {
                    Assert.IsTrue(cluster.RecordsCount >= 0);
                }
            }
        }

        [TestMethod]
        public void TestDatabaseClusterDataRange()
        {
            using (ODatabase database = new ODatabase(_hostname, _port, _databaseName, ODatabaseType.Document, _username, _password))
            {
                foreach (OCluster cluster in database.Clusters)
                {
                    long[] range = cluster.DataRange;

                    Assert.IsTrue(range[0] >= -1);
                    Assert.IsTrue(range[1] >= -1);
                }
            }
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
