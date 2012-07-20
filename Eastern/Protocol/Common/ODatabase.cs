using System.Collections.Generic;

namespace Eastern
{
    public class ODatabase
    {
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
    }
}
