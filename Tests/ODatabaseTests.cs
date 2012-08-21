using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Eastern;

namespace Tests
{
    [TestClass]
    public class ODatabaseTests : IDisposable
    {
        private OServer _connection;

        private const string _hostname = "127.0.0.1";
        private const int _port = 2424;
        private const string _rootName = "root";
        private const string _rootPassword = "9F696830A58E8187F6CC36674C666AE73E202DE3B0216C5B8BF8403C276CEE52";
        private const string _databaseName = "tempEasternUniqueTestDatabase0001x";
        private const string _username = "admin";
        private const string _password = "admin";

        public ODatabaseTests()
        {
            _connection = new OServer(_hostname, _port, _rootName, _rootPassword);

            // create test database
            _connection.CreateDatabase(_databaseName, ODatabaseType.Document, OStorageType.Local);
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
            _connection.DeleteDatabase(_databaseName);

            _connection.Close();
        }
    }
}
