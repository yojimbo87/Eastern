Database operations - ODatabase API
---

ODatabase class exposes public API for data and database manipulation.

ODatabase class API
---

**Constructor**

    public ODatabase(string alias)
    
Gets pre-initiated connection with the server instance from database pool.

    public ODatabase(string hostname, int port, string databaseName, ODatabaseType databaseType, string userName, string userPassword)
    
Initiates single dedicated connection with the server instance and retrieves specified database.

**Properties**

    public int SessionID { get; }
    
Represents ID of current session between client and server instance.

    public bool IsConnected { get; }
    
Indicates if underlying socket is connected to server instance.
    
    public string Name { get; set; }
    
Represents name of the database.
    
    public ODatabaseType Type { get; set; }
    
Represents type of the database.
    
    public short ClustersCount { get; set; }
    
Represents count of clusters within database.
    
    public List<OCluster> Clusters { get; set; }
    
List of clusters within database.
    
    public byte[] ClusterConfig { get; set; }
    
Represents cluster configuration in binary format.
    
    public long Size { get; }
    
Represents size of the database in bytes. Always retrieves most recent size when accessed.
    
    public long RecordsCount { get; }
    
Represents count of records within database. Always retrieves most recent count when accessed.
    
**Methods**

    public void Reload()

    public OCluster AddCluster(OClusterType type, string name)

    public OCluster AddCluster(OClusterType type, string name, string location, string dataSegmentName)

    public bool RemoveCluster(short clusterID)

    public int AddSegment(string name, string location)
    
    public bool RemoveSegment(string name)
    
    public void Close()

OCluster class
---

**Constructor**

    public OCluster()

**Properties**

    public short ID { get; set; }
    
    public string Name { get; set; }
    
    public OClusterType Type { get; set; }
    
    public string Location { get; set; }
    
    public short DataSegmentID { get; set; }
    
    public string DataSegmentName { get; set; }

    public long RecordsCount { get; }
    
    public long[] DataRange { get; }