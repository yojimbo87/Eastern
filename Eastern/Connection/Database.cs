using System;
using System.Linq;
using System.Collections.Generic;
using Eastern.Protocol;
using Eastern.Protocol.Operations;

namespace Eastern.Connection
{
    internal class Database : IDisposable
    {
        #region Properties

        internal Worker WorkerConnection { get; set; }
        internal bool ReturnToPool { get; set; }
        internal string Alias { get; set; }
        internal int SessionID { get { return WorkerConnection.SessionID; } }

        internal bool IsConnected
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

        internal string Name { get; set; }
        internal ODatabaseType Type { get; set; }
        internal short ClustersCount { get; set; }
        internal List<OCluster> Clusters { get; set; }
        internal byte[] ClusterConfig { get; set; }

        internal long Size
        {
            get
            {
                DbSize operation = new DbSize();

                return (long)WorkerConnection.ExecuteOperation<DbSize>(operation);
            }
        }

        internal long RecordsCount
        {
            get
            {
                DbCountRecords operation = new DbCountRecords();

                return (long)WorkerConnection.ExecuteOperation<DbCountRecords>(operation);
            }
        }

        #endregion

        internal Database(string hostname, int port, string databaseName, ODatabaseType databaseType, string userName, string userPassword)
        {
            WorkerConnection = new Worker();
            WorkerConnection.Initialize(hostname, port);
            ReturnToPool = false;

            DbOpen operation = new DbOpen();
            operation.DatabaseName = databaseName;
            operation.DatabaseType = databaseType;
            operation.UserName = userName;
            operation.UserPassword = userPassword;

            DtoDatabase dtoDatabase = (DtoDatabase)WorkerConnection.ExecuteOperation<DbOpen>(operation);

            WorkerConnection.SessionID = dtoDatabase.SessionID;
            Name = databaseName;
            Type = databaseType;
            ClustersCount = dtoDatabase.ClustersCount;
            Clusters = dtoDatabase.Clusters;
            ClusterConfig = dtoDatabase.ClusterConfig;

            // assign worker connection to each cluster since each instance of cluster can perform some operations
            foreach (OCluster cluster in Clusters)
            {
                cluster.WorkerConnection = WorkerConnection;
            }
        }

        public void Reload()
        {
            DbReload operation = new DbReload();
            DtoDatabase database = (DtoDatabase)WorkerConnection.ExecuteOperation<DbReload>(operation);

            ClustersCount = database.ClustersCount;
            Clusters = database.Clusters;

            // assign worker connection to each cluster since each instance of cluster can perform some operations
            foreach (OCluster cluster in Clusters)
            {
                cluster.WorkerConnection = WorkerConnection;
            }
        }

        #region Cluster methods

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

        #endregion

        #region Segment methods

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

        #endregion

        /*public ORecord CreateRecord<T>(T recordObject, bool isAsynchronous = false)
        {
            Type objectType = recordObject.GetType();

            OCluster cluster = Clusters.Where(o => o.Name == objectType.Name.ToLower()).FirstOrDefault();

            if (cluster == null)
            {
                cluster = AddCluster(OClusterType.Physical, objectType.Name.ToLower());
            }

            return CreateRecord(-1, cluster.ID, RecordParser.SerializeObject(recordObject, objectType), ORecordType.Document, isAsynchronous);
        }

        public ORecord CreateRecord<T>(string clusterName, T recordObject, bool isAsynchronous = false)
        {
            Type objectType = recordObject.GetType();

            OCluster cluster = Clusters.Where(o => o.Name == clusterName).FirstOrDefault();

            if (cluster == null)
            {
                cluster = AddCluster(OClusterType.Physical, clusterName);
            }

            return CreateRecord(-1, cluster.ID, RecordParser.SerializeObject(recordObject, objectType), ORecordType.Document, isAsynchronous);
        }*/

        public ORecord CreateRecord<T>(int segmentID, short clusterID, T recordObject, bool isAsynchronous)
        {
            Type objectType = recordObject.GetType();

            return CreateRecord(segmentID, clusterID, RecordParser.SerializeObject(recordObject, objectType), ORecordType.Document, isAsynchronous);
        }

        public ORecord CreateRecord(int segmentID, short clusterID, byte[] content, ORecordType type, bool isAsynchronous)
        {
            RecordCreate operation = new RecordCreate();
            operation.SegmentID = segmentID;
            operation.ClusterID = clusterID;
            operation.RecordContent = content;
            operation.RecordType = type;
            operation.OperationMode = (isAsynchronous) ? OperationMode.Asynchronous : OperationMode.Synchronous;

            return new ORecord((DtoRecord)WorkerConnection.ExecuteOperation<RecordCreate>(operation));
        }

        public int UpdateRecord(ORID orid, byte[] content, int version, ORecordType type, bool isAsynchronous)
        {
            RecordUpdate operation = new RecordUpdate();
            operation.ClusterID = orid.ClusterID;
            operation.ClusterPosition = orid.ClusterPosition;
            operation.RecordContent = content;
            operation.RecordVersion = version;
            operation.RecordType = type;
            operation.OperationMode = (isAsynchronous) ? OperationMode.Asynchronous : OperationMode.Synchronous;

            return (int)WorkerConnection.ExecuteOperation<RecordUpdate>(operation);
        }

        public bool DeleteRecord(ORID orid, int version, ORecordType type, bool isAsynchronous)
        {
            RecordDelete operation = new RecordDelete();
            operation.ClusterID = orid.ClusterID;
            operation.ClusterPosition = orid.ClusterPosition;
            operation.RecordVersion = version;
            operation.OperationMode = (isAsynchronous) ? OperationMode.Asynchronous : OperationMode.Synchronous;

            return (bool)WorkerConnection.ExecuteOperation<RecordDelete>(operation);
        }

        public ORecord LoadRecord(ORID orid, string fetchPlan, bool ignoreCache)
        {
            RecordLoad operation = new RecordLoad();
            operation.ClusterID = orid.ClusterID;
            operation.ClusterPosition = orid.ClusterPosition;
            operation.FetchPlan = fetchPlan;
            operation.IgnoreCache = ignoreCache;

            return ((DtoRecord)WorkerConnection.ExecuteOperation<RecordLoad>(operation)).Deserialize();
        }

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

        public void Dispose()
        {
            Close();
        }
    }
}
