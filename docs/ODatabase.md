Database operations API
---

Database (and data) operations are exposed through following public API.

ODatabase class
---

**Constructors**

    // Gets pre-initiated connection with the server instance from database pool.
    public ODatabase(string alias)

    // Initiates single dedicated connection with the server instance and retrieves specified database.
    public ODatabase(
        string hostname, 
        int port, 
        string databaseName, 
        ODatabaseType databaseType, 
        string userName, 
        string userPassword
    )

**Properties**

    // Represents ID of current session between client and server instance.
    public int SessionID { get; }

    // Indicates if underlying socket is connected to server instance.
    public bool IsConnected { get; }
    
    // Represents name of the database.
    public string Name { get; set; }
    
    // Represents type of the database.
    public ODatabaseType Type { get; set; }
    
    // Represents count of clusters within database.
    public short ClustersCount { get; set; }
    
    // List of clusters within database.
    public List<OCluster> Clusters { get; set; }
    
    // Represents cluster configuration in binary format.
    public byte[] ClusterConfig { get; set; }
    
    // Represents size of the database in bytes.
    // Always retrieves most recent size when accessed.
    public long Size { get; }
    
    // Represents count of records within database.
    // Always retrieves most recent count when accessed.
    public long RecordsCount { get; }
    
**Methods**

    // Reloads clusters and cluster count from server.
    public void Reload()
    
    // Adds new cluster to the database.
    public OCluster AddCluster(OClusterType type, string name)
    public OCluster AddCluster(OClusterType type, string name, string location, string dataSegmentName)

    // Removes cluster to the database. 
    // Returns boolean indicating if the specified cluster was successfuly removed.
    public bool RemoveCluster(short clusterID)

    // Adds new data segment to the database.
    // Returns ID of newly added data segment.
    public int AddSegment(string name, string location)
    
    // Removes specified data segment from database.
    // Returns boolean indicating if the specified data segment was successfuly removed.
    public bool RemoveSegment(string name)
    
    // Following methods create record within current database.
    // Returns ORecord object with assigned new record ID and version (returned only in synchronous mode). 
    // Methods are synchronous if isAsynchronous parameter isn't present.
    
    // If cluster with specified name doesn't exist, it's created.
    public ORecord CreateRecord<T>(string clusterName, T recordObject)
    public ORecord CreateRecord<T>(short clusterID, T recordObject)
    public ORecord CreateRecord<T>(short clusterID, T recordObject, bool isAsynchronous)
    public ORecord CreateRecord<T>(int segmentID, short clusterID, T recordObject, bool isAsynchronous)
    public ORecord CreateRecord(short clusterID, byte[] content, ORecordType type)
    public ORecord CreateRecord(int segmentID, short clusterID, byte[] content, ORecordType type, bool isAsynchronous)
    
    // Following methods update record within current database.
    // Returns integer indicating new record version (returned only in synchronous mode).
    // Methods without version parameter use version control.
    // Methods are synchronous if isAsynchronous parameter isn't present.
    
    public int UpdateRecord<T>(ORID orid, T recordObject)
    public int UpdateRecord<T>(ORID orid, T recordObject, bool isAsynchronous)
    public int UpdateRecord<T>(ORID orid, T recordObject, int version, bool isAsynchronous)
    public int UpdateRecord(ORID orid, byte[] content, int version, ORecordType type, bool isAsynchronous)
    
    // Following methods delete specific record within current database.
    // Returns boolean indicating if the record was deleted (returned only in synchronous mode).
    // Methods without type parameter deletes document type record.
    // Methods are synchronous if isAsynchronous parameter isn't present.
     
    public bool DeleteRecord(ORID orid)
    public bool DeleteRecord(ORID orid, int version)
    public bool DeleteRecord(ORID orid, bool isAsynchronous)
    public bool DeleteRecord(ORID orid, int version, bool isAsynchronous)
    public bool DeleteRecord(ORID orid, int version, ORecordType type, bool isAsynchronous)
    
    // Following methods load specific record from database.
    // Returns raw ORecord or specified class instance.
    // Methods without fetchPlan parameter use *.0 value by default.
    // Methods without ignoreCache parameter ignore cache by default.
    
    public T LoadRecord<T>(ORID orid)
    public T LoadRecord<T>(ORID orid, string fetchPlan)
    public T LoadRecord<T>(ORID orid, string fetchPlan, bool ignoreCache)
    public ORecord LoadRecord(ORID orid)
    public ORecord LoadRecord(ORID orid, string fetchPlan)
    public ORecord LoadRecord(ORID orid, string fetchPlan, bool ignoreCache)
    
    // Closes connection with server instance and resets session ID assigned to this object. 
    // This method is also called when ODatabase instance is being disposed.
    public void Close()

    // Closes connection with server instance and disposes ODatabase object.
    public void Dispose()

OCluster class
---

**Constructor**

    // Default contructor.
    public OCluster()

**Properties**

    // Represents ID of the cluster.
    public short ID { get; set; }
    
    // Represents name of the cluster.
    public string Name { get; set; }
    
    // Represents type of the cluster.
    public OClusterType Type { get; set; }
    
    // Represents location of the cluster.
    public string Location { get; set; }
    
    // Represents data segment ID of the cluster.
    public short DataSegmentID { get; set; }
    
    // Represents data segment name of the cluster.
    public string DataSegmentName { get; set; }

    // Represents count of records the cluster.
    // Always retrieves most recent count when accessed.
    public long RecordsCount { get; }
    
    // Represents range (min and max value) of record IDs in the cluster.
    // Always retrieves most recent range when accessed.
    public long[] DataRange { get; }

ODatabaseType enum
---

    Document = 0
    Graph = 1
    
OClusterType enum
---

    Physical = 0
    Memory = 1