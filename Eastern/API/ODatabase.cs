using System.Collections.Generic;
using Eastern.Connection;
using Eastern.Protocol;
using Eastern.Protocol.Operations;

namespace Eastern
{
    public class ODatabase
    {
        internal Worker WorkerConnection { get; set; }

        public int SessionID { get { return WorkerConnection.SessionID; } }
        public string Name { get; set; }
        public ODatabaseType Type { get; set; }
        public short ClustersCount { get; set; }
        public List<OCluster> Clusters { get; set; }
        public byte[] ClusterConfig { get; set; }

        public long Size
        {
            get
            {
                DbSize operation = new DbSize();

                return (long)WorkerConnection.ExecuteOperation<DbSize>(operation);
            }
        }

        public long RecordsCount
        {
            get
            {
                DbCountRecords operation = new DbCountRecords();

                return (long)WorkerConnection.ExecuteOperation<DbCountRecords>(operation);
            }
        }

        public ODatabase(string hostname, int port, string databaseName, ODatabaseType databaseType, string userName, string userPassword)
        {
            WorkerConnection = new Worker();
            WorkerConnection.Initialize(hostname, port);

            DbOpen operation = new DbOpen();
            operation.DatabaseName = databaseName;
            operation.DatabaseType = databaseType;
            operation.UserName = userName;
            operation.UserPassword = userPassword;

            Database database = (Database)WorkerConnection.ExecuteOperation<DbOpen>(operation);

            WorkerConnection.SessionID = database.SessionID;
            Name = database.Name;
            Type = database.Type;
            ClustersCount = database.ClustersCount;
            Clusters = database.Clusters;
            ClusterConfig = database.ClusterConfig;
        }

        public void Reload()
        {
            DbReload operation = new DbReload();
            Database database = (Database)WorkerConnection.ExecuteOperation<DbReload>(operation);

            ClustersCount = database.ClustersCount;
            Clusters = database.Clusters;
        }

        public OCluster AddCluster(OClusterType type, string name)
        {
            return AddCluster(type, name, "default", "default");
        }

        public OCluster AddCluster(OClusterType type, string name, string location, string dataSegmentName)
        {
            DataClusterAdd operation = new DataClusterAdd();
            operation.Type = type;
            operation.Name = name;
            operation.Location = location;
            operation.DataSegmentName = dataSegmentName;

            short clusterID = (short)WorkerConnection.ExecuteOperation<DataClusterAdd>(operation);

            OCluster cluster = new OCluster();
            cluster.WorkerConnection = WorkerConnection;
            cluster.ID = clusterID;
            cluster.Type = type;
            cluster.Name = name;
            cluster.Location = location;
            cluster.DataSegmentName = dataSegmentName;

            Clusters.Add(cluster);
            ClustersCount++;

            return cluster;
        }

        // return value indicated if the cluster was successfuly removed
        public bool RemoveCluster(short clusterID)
        {
            DataClusterRemove operation = new DataClusterRemove();
            operation.ClusterID = clusterID;

            byte deleteOnClientSide = (byte)WorkerConnection.ExecuteOperation<DataClusterRemove>(operation);

            if (deleteOnClientSide == 1)
            {
                OCluster cluster = Clusters.Find(q => q.ID == clusterID);

                if (cluster != null)
                {
                    Clusters.Remove(cluster);
                    ClustersCount--;

                    return true;
                }
            }

            return false;
        }

        public int AddSegment(string name, string location)
        {
            DataSegmentAdd operation = new DataSegmentAdd();
            operation.SegmentName = name;
            operation.SegmentLocation = location;

            return (int)WorkerConnection.ExecuteOperation<DataSegmentAdd>(operation);
        }

        public bool RemoveSegment(string name)
        {
            DataSegmentRemove operation = new DataSegmentRemove();
            operation.SegmentName = name;

            return (bool)WorkerConnection.ExecuteOperation<DataSegmentRemove>(operation);
        }

        public void Close()
        {
            DbClose operation = new DbClose();

            WorkerConnection.ExecuteOperation<DbClose>(operation);
            WorkerConnection.SessionID = -1;
            WorkerConnection.Close();
        }
    }
}
