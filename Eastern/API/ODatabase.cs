using System;
using System.Collections.Generic;
using Eastern.Connection;
using Eastern.Protocol;
using Eastern.Protocol.Operations;

namespace Eastern
{
    public class ODatabase : IDisposable
    {
        internal Worker WorkerConnection { get; set; }
        internal bool ReturnToPool { get; set; }
        internal string Hash { get; set; }

        /// <summary>
        /// Represents ID of current session between client and server instance.
        /// </summary>
        public int SessionID { get { return WorkerConnection.SessionID; } }

        /// <summary>
        /// Indicates if underlying socket is connected to server instance.
        /// </summary>
        public bool IsConnected
        {
            get
            {
                if ((WorkerConnection != null) && WorkerConnection.IsConnected)
                {
                    return true;
                }

                return false;
            }
        }

        /// <summary>
        /// Represents name of the database.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Represents type of the database.
        /// </summary>
        public ODatabaseType Type { get; set; }

        /// <summary>
        /// Represents count of clusters within database.
        /// </summary>
        public short ClustersCount { get; set; }

        /// <summary>
        /// List of clusters within database.
        /// </summary>
        public List<OCluster> Clusters { get; set; }

        /// <summary>
        /// Represents cluster configuration in binary format.
        /// </summary>
        public byte[] ClusterConfig { get; set; }

        /// <summary>
        /// Represents size of the database in bytes. Always retrieves most recent size when accessed.
        /// </summary>
        public long Size
        {
            get
            {
                DbSize operation = new DbSize();

                return (long)WorkerConnection.ExecuteOperation<DbSize>(operation);
            }
        }

        /// <summary>
        /// Represents count of records within database. Always retrieves most recent count when accessed.
        /// </summary>
        public long RecordsCount
        {
            get
            {
                DbCountRecords operation = new DbCountRecords();

                return (long)WorkerConnection.ExecuteOperation<DbCountRecords>(operation);
            }
        }

        /// <summary>
        /// Initiates single dedicated connection with the server instance and retrieves specified database.
        /// </summary>
        public ODatabase(string hostname, int port, string databaseName, ODatabaseType databaseType, string userName, string userPassword)
        {
            WorkerConnection = new Worker();
            WorkerConnection.Initialize(hostname, port);
            ReturnToPool = false;
            Hash = hostname + port + databaseName + databaseType.ToString() + userName;

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

            // assign worker connection to each cluster since each instance of cluster can perform some operations
            foreach (OCluster cluster in Clusters)
            {
                cluster.WorkerConnection = WorkerConnection;
            }
        }

        /// <summary>
        /// Reloads clusters and cluster count from server.
        /// </summary>
        public void Reload()
        {
            DbReload operation = new DbReload();
            Database database = (Database)WorkerConnection.ExecuteOperation<DbReload>(operation);

            ClustersCount = database.ClustersCount;
            Clusters = database.Clusters;

            // assign worker connection to each cluster since each instance of cluster can perform some operations
            foreach (OCluster cluster in Clusters)
            {
                cluster.WorkerConnection = WorkerConnection;
            }
        }

        /// <summary>
        /// Adds new cluster to the database.
        /// </summary>
        public OCluster AddCluster(OClusterType type, string name)
        {
            return AddCluster(type, name, "default", "default");
        }

        /// <summary>
        /// Adds new cluster to the database.
        /// </summary>
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

        /// <summary>
        /// Adds new cluster to the database.
        /// </summary>
        /// <returns>
        /// Boolean indicating if the specified cluster was successfuly removed.
        /// </returns>
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

        /// <summary>
        /// Adds new data segment to the database.
        /// </summary>
        /// <returns>
        /// ID of newly added data segment.
        /// </returns>
        public int AddSegment(string name, string location)
        {
            DataSegmentAdd operation = new DataSegmentAdd();
            operation.SegmentName = name;
            operation.SegmentLocation = location;

            return (int)WorkerConnection.ExecuteOperation<DataSegmentAdd>(operation);
        }

        /// <summary>
        /// Removes specified data segment from database.
        /// </summary>
        /// <returns>
        /// Boolean indicating if the specified data segment was successfuly removed.
        /// </returns>
        public bool RemoveSegment(string name)
        {
            DataSegmentRemove operation = new DataSegmentRemove();
            operation.SegmentName = name;

            return (bool)WorkerConnection.ExecuteOperation<DataSegmentRemove>(operation);
        }

        /// <summary>
        /// Creates record within current database.
        /// </summary>
        /// <returns>
        /// ORecord object.
        /// </returns>
        public ORecord CreateRecord(int segmentID, short clusterID, byte[] content, ORecordType type, bool isAsynchronous)
        {
            RecordCreate operation = new RecordCreate();
            operation.SegmentID = segmentID;
            operation.ClusterID = clusterID;
            operation.RecordContent = content;
            operation.RecordType = type;
            operation.OperationMode = (isAsynchronous) ? OperationMode.Asynchronous : OperationMode.Synchronous;

            return new ORecord((Record)WorkerConnection.ExecuteOperation<RecordCreate>(operation));
        }

        /// <summary>
        /// Load specific record from database.
        /// </summary>
        /// <returns>
        /// ORecord object.
        /// </returns>
        public ORecord LoadRecord(short clusterID, long clusterPosition, string fetchPlan, bool ignoreCache)
        {
            RecordLoad operation = new RecordLoad();
            operation.ClusterID = clusterID;
            operation.ClusterPosition = clusterPosition;
            operation.FetchPlan = fetchPlan;
            operation.IgnoreCache = ignoreCache;

            return new ORecord((Record)WorkerConnection.ExecuteOperation<RecordLoad>(operation));
        }

        /// <summary>
        /// Closes connection with server instance and resets session ID assigned to this object.
        /// </summary>
        public void Close()
        {
            if (ReturnToPool)
            {
                EasternClient.ReturnDatabase(this);
            }
            else
            {
                DbClose operation = new DbClose();

                WorkerConnection.ExecuteOperation<DbClose>(operation);
                WorkerConnection.SessionID = -1;
                WorkerConnection.Close();
            }
        }

        /// <summary>
        /// Closes connection with server instance and disposes this object.
        /// </summary>
        public void Dispose()
        {
            Close();
        }
    }
}
