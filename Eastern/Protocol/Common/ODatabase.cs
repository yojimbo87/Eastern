using System.Collections.Generic;
using Eastern.Connection;
using Eastern.Protocol.Operations;

namespace Eastern
{
    public class ODatabase
    {
        internal WorkerConnection WorkerConnection { get; set; }

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

            ClustersCount = database.ClustersCount;
            Clusters = database.Clusters;
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
