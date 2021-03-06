﻿using System.Collections.Generic;

namespace Eastern.Protocol
{
    internal class DtoDatabase
    {
        internal int SessionID { get; set; }
        internal string Name { get; set; }
        internal ODatabaseType Type { get; set; }
        internal short ClustersCount { get; set; }
        internal List<OCluster> Clusters { get; set; }
        internal byte[] ClusterConfig { get; set; }

        internal DtoDatabase()
        {
            Clusters = new List<OCluster>();
        }
    }
}
