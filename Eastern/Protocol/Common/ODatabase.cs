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

        public ODatabase()
        {
            Clusters = new List<OCluster>();
        }

        // return value indicates if the database was created successfuly
        public bool Close()
        {
            CloseDatabase operation = new CloseDatabase();

            bool isDatabaseClosed = (bool)WorkerConnection.ExecuteOperation<CloseDatabase>(operation);

            if (isDatabaseClosed)
            {
                WorkerConnection.Close();
            }

            return isDatabaseClosed;
        }
    }
}
