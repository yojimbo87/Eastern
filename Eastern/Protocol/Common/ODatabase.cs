using System.Collections.Generic;
using Eastern.Connection;
using Eastern.Protocol.Operations;

namespace Eastern
{
    public class ODatabase
    {
        internal Worker WorkerConnection { get; set; }

        public int SessionID { get; set; }
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

        public ODatabase()
        {
            Clusters = new List<OCluster>();
        }

        public void Reload()
        {
            DbReload operation = new DbReload();
            ODatabase database = (ODatabase)WorkerConnection.ExecuteOperation<DbReload>(operation);

            // add worker connection to each cluster
            foreach (OCluster cluster in database.Clusters)
            {
                cluster.WorkerConnection = WorkerConnection;
            }

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

                    return true;
                }
            }

            return false;
        }

        public void Close()
        {
            DbClose operation = new DbClose();

            WorkerConnection.ExecuteOperation<DbClose>(operation);
            WorkerConnection.Close();
            SessionID = -1;
        }
    }
}
