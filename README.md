Eastern is a C#/.NET driver for [OrientDB](http://code.google.com/p/orient/) which implements [network binary protocol](http://code.google.com/p/orient/wiki/NetworkBinaryProtocol) to offer the fastest way for communication between .NET based applications and OrientDB server instances.

Status
---

Currently in early stage of development.

Features
---

- Initial connection with database server
- Request/response parsing
- Request/response error handling
- Supported operations:
  - Shutdown (SHUTDOWN)
  - Connect (CONNECT)
  - Open database (DB_OPEN)
  - Create database (DB_CREATE)
  - Close connection/database (DB_CLOSE)
  - Database exist check (DB_EXIST)
  - Reload database (DB_RELOAD)
  - Delete database (DB_DELETE)
  - Database size (DB_SIZE)
  - Database records count (DB_COUNTRECORDS)
  - Add cluster to database (DATACLUSTER_ADD)
  - Remove cluster from database (DATACLUSTER_REMOVE)
  - Count of records in range of clusters (DATACLUSTER_COUNT)
  - Data range of records in cluster (DATACLUSTER_DATARANGE)
  - Count of records in cluster (COUNT)
  - Add data segment (DATASEGMENT_ADD)
  - Remove data segment (DATASEGMENT_REMOVE)

Public API
===

OServer class
---

**Constructor**

    public OServer(string hostname, int port, string userName, string userPassword)

**Properties**

    public int SessionID { get; }

    public bool IsConnected { get; }
    
**Methods**

    public bool Shutdown()

    public bool CreateDatabase(string databaseName, ODatabaseType databaseType, OStorageType storageType)

    public bool DatabaseExist(string databaseName)

    public void DeleteDatabase(string databaseName)

    public void Close()

ODatabase class
---

**Constructor**

    public ODatabase(string hostname, int port, string databaseName, ODatabaseType databaseType, string userName, string userPassword)

**Properties**

    public int SessionID { get; }

    public bool IsConnected { get; }
    
    public string Name { get; set; }
    
    public ODatabaseType Type { get; set; }
    
    public short ClustersCount { get; set; }
    
    public List<OCluster> Clusters { get; set; }
    
    public byte[] ClusterConfig { get; set; }
    
    public long Size { get; }
    
    public long RecordsCount { get; }
    
**Methods**

    public void Reload()

    public OCluster AddCluster(OClusterType type, string name)

    public OCluster AddCluster(OClusterType type, string name, string location, string dataSegmentName)

    public bool RemoveCluster(short clusterID)

    public int AddSegment(string name, string location)
    
    public bool RemoveSegment(string name)
    
    public void Close()
    
TODO
---

- Wiki with API usage
- Support for client side connection pooling
  - Implementation similar to [SQL Server connection pooling](http://msdn.microsoft.com/en-us/library/8xx3tyca.aspx)
  - Examples [here](http://stackoverflow.com/questions/1148467/is-there-a-standard-way-of-implementing-a-proprietary-connection-pool-in-net) and [here](http://www.codeproject.com/Articles/35011/NET-TCP-Connection-Pooling)
  - Unique pool for each database based on connection string or configuration
  - Automatic reconnection of pooled connection with retry timer
  - Queuing of operations when there is no free connection within the pool
- Support entire set of [operations](http://code.google.com/p/orient/wiki/NetworkBinaryProtocol#Operations)