﻿using System.Collections.Generic;
using Eastern.Connection;
using Eastern.Protocol.Operations;

namespace Eastern
{
    public class OCluster
    {
        internal Worker WorkerConnection { get; set; }

        public short ID { get; set; }
        public string Name { get; set; }
        public OClusterType Type { get; set; }
        public string Location { get; set; }
        public short DataSegmentID { get; set; }
        public string DataSegmentName { get; set; }

        public long RecordsCount
        {
            get
            {
                Count operation = new Count();
                operation.ClusterName = Name;

                return (long)WorkerConnection.ExecuteOperation<Count>(operation);
            }
        }

        public long[] DataRange
        {
            get
            {
                DataClusterDataRange operation = new DataClusterDataRange();
                operation.ClusterID = ID;

                return (long[])WorkerConnection.ExecuteOperation<DataClusterDataRange>(operation);
            }
        }
    }
}