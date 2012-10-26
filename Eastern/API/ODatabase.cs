using System;
using System.Linq;
using System.Threading;
using System.Collections.Generic;
using Eastern.Connection;
using Eastern.Protocol;
using Eastern.Protocol.Operations;

namespace Eastern
{
    public class ODatabase : IDisposable
    {
        private Database _database { get; set; }

        #region Public properties

        /// <summary>
        /// Represents ID of current session between client and server instance.
        /// </summary>
        public int SessionID { get { return _database.SessionID; } }

        /// <summary>
        /// Indicates if underlying socket is connected to server instance.
        /// </summary>
        public bool IsConnected { get { return _database.IsConnected; } }

        /// <summary>
        /// Represents name of the database.
        /// </summary>
        public string Name { get { return _database.Name; } }

        /// <summary>
        /// Represents type of the database.
        /// </summary>
        public ODatabaseType Type { get { return _database.Type; } }

        /// <summary>
        /// Represents count of clusters within database.
        /// </summary>
        public short ClustersCount { get { return _database.ClustersCount; } }

        /// <summary>
        /// List of clusters within database.
        /// </summary>
        public List<OCluster> Clusters { get { return _database.Clusters; } }

        /// <summary>
        /// Represents cluster configuration in binary format.
        /// </summary>
        public byte[] ClusterConfig { get { return _database.ClusterConfig; } }

        /// <summary>
        /// Represents size of the database in bytes. Always retrieves most recent size when accessed.
        /// </summary>
        public long Size { get { return _database.Size; } }

        /// <summary>
        /// Represents count of records within database. Always retrieves most recent count when accessed.
        /// </summary>
        public long RecordsCount { get { return _database.RecordsCount; } }

        #endregion

        /// <summary>
        /// Gets pre-initiated connection with the server instance from database pool.
        /// </summary>
        public ODatabase(string alias)
        {
            _database = EasternClient.GetDatabase(alias);
        }

        /// <summary>
        /// Initiates single dedicated connection with the server instance and retrieves specified database.
        /// </summary>
        public ODatabase(string hostname, int port, string databaseName, ODatabaseType databaseType, string userName, string userPassword)
        {
            _database = new Database(hostname, port, databaseName, databaseType, userName, userPassword);
        }

        /// <summary>
        /// Reloads clusters and cluster count from server.
        /// </summary>
        public void Reload()
        {
            _database.Reload();
        }

        #region Cluster methods

        /// <summary>
        /// Adds new cluster to the database.
        /// </summary>
        public OCluster AddCluster(OClusterType type, string name)
        {
            return _database.AddCluster(type, name, "default", "default");
        }

        /// <summary>
        /// Adds new cluster to the database.
        /// </summary>
        public OCluster AddCluster(OClusterType type, string name, string location, string dataSegmentName)
        {
            return _database.AddCluster(type, name, location, dataSegmentName);
        }

        /// <summary>
        /// Adds new cluster to the database.
        /// </summary>
        /// <returns>
        /// Boolean indicating if the specified cluster was successfuly removed.
        /// </returns>
        public bool RemoveCluster(short clusterID)
        {
            return _database.RemoveCluster(clusterID);
        }

        #endregion

        #region Segment methods

        /// <summary>
        /// Adds new data segment to the database.
        /// </summary>
        /// <returns>
        /// ID of newly added data segment.
        /// </returns>
        public int AddSegment(string name, string location)
        {
            return _database.AddSegment(name, location);
        }

        /// <summary>
        /// Removes specified data segment from database.
        /// </summary>
        /// <returns>
        /// Boolean indicating if the specified data segment was successfuly removed.
        /// </returns>
        public bool RemoveSegment(string name)
        {
            return _database.RemoveSegment(name);
        }

        #endregion

        #region Create record methods

        /// <summary>
        /// Creates record within current database. If cluster with specified name doesn't exist, it's created.
        /// </summary>
        /// <returns>
        /// ORecord object with assigned new record ID and version.
        /// </returns>
        public ORecord CreateRecord<T>(string clusterName, T recordObject)
        {
            // clusters are stored in lower case
            clusterName = clusterName.ToLower();

            OCluster cluster = Clusters.Where(o => o.Name == clusterName).FirstOrDefault();

            if (cluster == null)
            {
                // cluster cannot be found, so do database reload to see if it wasn't created in the mean time
                Reload();

                cluster = Clusters.Where(o => o.Name == clusterName).FirstOrDefault();

                if (cluster == null)
                {
                    // create new cluster if it isn't present in the DB after reload
                    cluster = AddCluster(OClusterType.Physical, clusterName);
                }
            }

            return CreateRecord<T>(-1, cluster.ID, recordObject, false);
        }

        /// <summary>
        /// Creates record within current database.
        /// </summary>
        /// <returns>
        /// ORecord object with assigned new record ID and version.
        /// </returns>
        public ORecord CreateRecord<T>(short clusterID, T recordObject)
        {
            return CreateRecord<T>(-1, clusterID, recordObject, false);
        }

        /// <summary>
        /// Creates record within current database.
        /// </summary>
        /// <returns>
        /// ORecord object with assigned new record ID and version (returned only in synchronous mode).
        /// </returns>
        public ORecord CreateRecord<T>(short clusterID, T recordObject, bool isAsynchronous)
        {
            return CreateRecord<T>(-1, clusterID, recordObject, isAsynchronous);
        }

        /// <summary>
        /// Creates record within current database.
        /// </summary>
        /// <returns>
        /// ORecord object with assigned new record ID and version (returned only in synchronous mode).
        /// </returns>
        public ORecord CreateRecord<T>(int segmentID, short clusterID, T recordObject, bool isAsynchronous)
        {
            return _database.CreateRecord<T>(segmentID, clusterID, recordObject, isAsynchronous);
        }

        /// <summary>
        /// Creates record within current database.
        /// </summary>
        /// <returns>
        /// ORecord object with assigned new record ID and version.
        /// </returns>
        public ORecord CreateRecord(short clusterID, byte[] content, ORecordType type)
        {
            return CreateRecord(-1, clusterID, content, type, false);
        }

        /// <summary>
        /// Creates record within current database.
        /// </summary>
        /// <returns>
        /// ORecord object with assigned new record ID and version (returned only in synchronous mode).
        /// </returns>
        public ORecord CreateRecord(int segmentID, short clusterID, byte[] content, ORecordType type, bool isAsynchronous)
        {
            return _database.CreateRecord(segmentID, clusterID, content, type, isAsynchronous);
        }

        #endregion

        #region Update record methods

        /// <summary>
        /// Updates record within current database with version control.
        /// </summary>
        /// <returns>
        /// Integer indicating new record version.
        /// </returns>
        public int UpdateRecord<T>(ORID orid, T recordObject)
        {
            return UpdateRecord<T>(orid, recordObject, 0, false);
        }

        /// <summary>
        /// Updates record within current database with version control.
        /// </summary>
        /// <returns>
        /// Integer indicating new record version (returned only in synchronous mode).
        /// </returns>
        public int UpdateRecord<T>(ORID orid, T recordObject, bool isAsynchronous)
        {
            return UpdateRecord<T>(orid, recordObject, 0, isAsynchronous);
        }

        /// <summary>
        /// Updates record within current database.
        /// </summary>
        /// <returns>
        /// Integer indicating new record version (returned only in synchronous mode).
        /// </returns>
        public int UpdateRecord<T>(ORID orid, T recordObject, int version, bool isAsynchronous)
        {
            return _database.UpdateRecord(orid, recordObject, version, isAsynchronous);
        }

        /// <summary>
        /// Updates record within current database.
        /// </summary>
        /// <returns>
        /// Integer indicating new record version (returned only in synchronous mode).
        /// </returns>
        public int UpdateRecord(ORID orid, byte[] content, int version, ORecordType type, bool isAsynchronous)
        {
            return _database.UpdateRecord(orid, content, version, type, isAsynchronous);
        }

        #endregion

        #region Delete record methods

        /// <summary>
        /// Deletes record within current database.
        /// </summary>
        /// <returns>
        /// Boolean indicating if the record was deleted (returned only in synchronous mode).
        /// </returns>
        public bool DeleteRecord(ORID orid, int version, ORecordType type, bool isAsynchronous)
        {
            return _database.DeleteRecord(orid, version, type, isAsynchronous);
        }

        #endregion

        #region Load record methods

        /// <summary>
        /// Load specific record from database.
        /// </summary>
        /// <returns>
        /// POCO mapped from ORecord.
        /// </returns>
        public T LoadRecord<T>(ORID orid) where T : class, new()
        {
            return LoadRecord(orid, "*:0", true).ToObject<T>();
        }

        /// <summary>
        /// Load specific record from database.
        /// </summary>
        /// <returns>
        /// POCO mapped from ORecord.
        /// </returns>
        public T LoadRecord<T>(ORID orid, string fetchPlan) where T : class, new()
        {
            return LoadRecord(orid, fetchPlan, true).ToObject<T>();
        }

        /// <summary>
        /// Load specific record from database.
        /// </summary>
        /// <returns>
        /// POCO mapped from ORecord.
        /// </returns>
        public T LoadRecord<T>(ORID orid, string fetchPlan, bool ignoreCache) where T : class, new()
        {
            return LoadRecord(orid, fetchPlan, ignoreCache).ToObject<T>();
        }

        /// <summary>
        /// Load specific record from database.
        /// </summary>
        /// <returns>
        /// ORecord object.
        /// </returns>
        public ORecord LoadRecord(ORID orid)
        {
            return LoadRecord(orid, "*:0", true);
        }
 
        /// <summary>
        /// Load specific record from database.
        /// </summary>
        /// <returns>
        /// ORecord object.
        /// </returns>
        public ORecord LoadRecord(ORID orid, string fetchPlan)
        {
            return LoadRecord(orid, fetchPlan, true);
        }

        /// <summary>
        /// Load specific record from database.
        /// </summary>
        /// <returns>
        /// ORecord object.
        /// </returns>
        public ORecord LoadRecord(ORID orid, string fetchPlan, bool ignoreCache)
        {
            return _database.LoadRecord(orid, fetchPlan, ignoreCache);
        }

        #endregion

        /// <summary>
        /// Closes connection with server instance and resets session ID assigned to this object.
        /// </summary>
        public void Close()
        {
            _database.Close();
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
