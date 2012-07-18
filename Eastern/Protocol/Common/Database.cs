using System.Collections.Generic;

namespace Eastern
{
    public class Database
    {
        public int SessionID { get; set; }
        public string Name { get; set; }
        public DatabaseType Type { get; set; }
        public short ClustersCount { get; set; }
        public List<Cluster> Clusters { get; set; }
        public byte[] ClusterConfig { get; set; }

        public Database()
        {
            Clusters = new List<Cluster>();
        }
    }
}
