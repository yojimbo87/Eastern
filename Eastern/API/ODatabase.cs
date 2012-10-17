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
        private Database Database { get; set; }
        internal bool ReturnToPool { get { return Database.ReturnToPool; } set { Database.ReturnToPool = value; } }

        #region Public properties

        /// <summary>
        /// Represents ID of current session between client and server instance.
        /// </summary>
        public int SessionID { get { return Database.SessionID; } }

        /// <summary>
        /// Indicates if underlying socket is connected to server instance.
        /// </summary>
        public bool IsConnected { get { return Database.IsConnected; } }

        /// <summary>
        /// Represents name of the database.
        /// </summary>
        public string Name { get { return Database.Name; } }

        /// <summary>
        /// Represents type of the database.
        /// </summary>
        public ODatabaseType Type { get { return Database.Type; } }

        /// <summary>
        /// Represents count of clusters within database.
        /// </summary>
        public short ClustersCount { get { return Database.ClustersCount; } }

        /// <summary>
        /// List of clusters within database.
        /// </summary>
        public List<OCluster> Clusters { get { return Database.Clusters; } }

        /// <summary>
        /// Represents cluster configuration in binary format.
        /// </summary>
        public byte[] ClusterConfig { get { return Database.ClusterConfig; } }

        /// <summary>
        /// Represents size of the database in bytes. Always retrieves most recent size when accessed.
        /// </summary>
        public long Size { get { return Database.Size; } }

        /// <summary>
        /// Represents count of records within database. Always retrieves most recent count when accessed.
        /// </summary>
        public long RecordsCount { get { return Database.RecordsCount; } }

        #endregion

        /// <summary>
        /// Gets pre-initiated connection with the server instance from database pool.
        /// </summary>
        public ODatabase(string alias)
        {
            Database = EasternClient.GetDatabase(alias);
        }

        /// <summary>
        /// Initiates single dedicated connection with the server instance and retrieves specified database.
        /// </summary>
        public ODatabase(string hostname, int port, string databaseName, ODatabaseType databaseType, string userName, string userPassword)
        {
            Database = new Database(hostname, port, databaseName, databaseType, userName, userPassword);
        }

        /// <summary>
        /// Reloads clusters and cluster count from server.
        /// </summary>
        public void Reload()
        {
            Database.Reload();
        }

        #region Cluster methods

        /// <summary>
        /// Adds new cluster to the database.
        /// </summary>
        public OCluster AddCluster(OClusterType type, string name)
        {
            return Database.AddCluster(type, name, "default", "default");
        }

        /// <summary>
        /// Adds new cluster to the database.
        /// </summary>
        public OCluster AddCluster(OClusterType type, string name, string location, string dataSegmentName)
        {
            return Database.AddCluster(type, name, location, dataSegmentName);
        }

        /// <summary>
        /// Adds new cluster to the database.
        /// </summary>
        /// <returns>
        /// Boolean indicating if the specified cluster was successfuly removed.
        /// </returns>
        public bool RemoveCluster(short clusterID)
        {
            return Database.RemoveCluster(clusterID);
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
            return Database.AddSegment(name, location);
        }

        /// <summary>
        /// Removes specified data segment from database.
        /// </summary>
        /// <returns>
        /// Boolean indicating if the specified data segment was successfuly removed.
        /// </returns>
        public bool RemoveSegment(string name)
        {
            return Database.RemoveSegment(name);
        }

        #endregion

        #region Create record methods

        /// <summary>
        /// Creates record within current database.
        /// </summary>
        /// <returns>
        /// ORecord object with assigned new record ID and version (returned only in synchronous mode).
        /// </returns>
        public ORecord CreateRecord<T>(T recordObject, bool isAsynchronous = false)
        {
            return Database.CreateRecord<T>(recordObject, isAsynchronous);
        }

        /// <summary>
        /// Creates record within current database.
        /// </summary>
        /// <returns>
        /// ORecord object with assigned new record ID and version (returned only in synchronous mode).
        /// </returns>
        public ORecord CreateRecord<T>(string clusterName, T recordObject, bool isAsynchronous = false)
        {
            return Database.CreateRecord<T>(clusterName, recordObject, isAsynchronous);
        }

        /// <summary>
        /// Creates record within current database.
        /// </summary>
        /// <returns>
        /// ORecord object with assigned new record ID and version (returned only in synchronous mode).
        /// </returns>
        public ORecord CreateRecord<T>(short clusterID, T recordObject, bool isAsynchronous = false)
        {
            return Database.CreateRecord<T>(clusterID, recordObject, isAsynchronous);
        }

        /// <summary>
        /// Creates record within current database.
        /// </summary>
        /// <returns>
        /// ORecord object with assigned new record ID and version (returned only in synchronous mode).
        /// </returns>
        public ORecord CreateRecord(short clusterID, byte[] content, ORecordType type, bool isAsynchronous = false)
        {
            return Database.CreateRecord(clusterID, content, type, isAsynchronous);
        }

        /// <summary>
        /// Creates record within current database.
        /// </summary>
        /// <returns>
        /// ORecord object with assigned new record ID and version (returned only in synchronous mode).
        /// </returns>
        public ORecord CreateRecord(int segmentID, short clusterID, byte[] content, ORecordType type, bool isAsynchronous = false)
        {
            return Database.CreateRecord(segmentID, clusterID, content, type, isAsynchronous);
        }

        #endregion

        #region Update record methods

        /// <summary>
        /// Updates record within current database.
        /// </summary>
        /// <returns>
        /// Integer indicating new record version (returned only in synchronous mode).
        /// </returns>
        public int UpdateRecord(ORID orid, byte[] content, int version, ORecordType type, bool isAsynchronous)
        {
            return Database.UpdateRecord(orid, content, version, type, isAsynchronous);
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
            return Database.DeleteRecord(orid, version, type, isAsynchronous);
        }

        #endregion

        #region Load record methods

        /// <summary>
        /// Load specific record from database.
        /// </summary>
        /// <returns>
        /// Generic object.
        /// </returns>
        public T LoadRecord<T>(ORID orid, string fetchPlan = "*:0") where T : class, new()
        {
            return Database.LoadRecord<T>(orid, fetchPlan);
        }

        /// <summary>
        /// Load specific record from database.
        /// </summary>
        /// <returns>
        /// ORecord object.
        /// </returns>
        public ORecord LoadRecord(ORID orid, string fetchPlan = "*:0")
        {
            return Database.LoadRecord(orid, fetchPlan, true);
        }

        /// <summary>
        /// Load specific record from database.
        /// </summary>
        /// <returns>
        /// ORecord object.
        /// </returns>
        public ORecord LoadRecord(ORID orid, string fetchPlan, bool ignoreCache)
        {
            return Database.LoadRecord(orid, fetchPlan, ignoreCache);
        }

        #endregion

        /// <summary>
        /// Closes connection with server instance and resets session ID assigned to this object.
        /// </summary>
        public void Close()
        {
            Database.Close();
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
