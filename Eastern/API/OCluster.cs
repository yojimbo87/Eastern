using System.Collections.Generic;
using Eastern.Connection;
using Eastern.Protocol.Operations;

namespace Eastern
{
    public class OCluster
    {
        internal Worker WorkerConnection { get; set; }

        /// <summary>
        /// Represents ID of the cluster.
        /// </summary>
        public short ID { get; set; }

        /// <summary>
        /// Represents name of the cluster.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Represents type of the cluster.
        /// </summary>
        public OClusterType Type { get; set; }

        /// <summary>
        /// Represents location of the cluster.
        /// </summary>
        public string Location { get; set; }

        /// <summary>
        /// Represents data segment ID of the cluster.
        /// </summary>
        public short DataSegmentID { get; set; }

        /// <summary>
        /// Represents data segment name of the cluster.
        /// </summary>
        public string DataSegmentName { get; set; }

        /// <summary>
        /// Represents count of records the cluster. Always retrieves most recent count when accessed.
        /// </summary>
        public long RecordsCount
        {
            get
            {
                Count operation = new Count();
                operation.ClusterName = Name;

                return (long)WorkerConnection.ExecuteOperation<Count>(operation);
            }
        }

        /// <summary>
        /// Represents range of record IDs in the cluster. Always retrieves most recent range when accessed.
        /// </summary>
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
