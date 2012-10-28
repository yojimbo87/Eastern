Database operations - ODatabase API
---

ODatabase class exposes public API for data and database manipulation.

ODatabase class
---

**Constructor**

    // Gets pre-initiated connection with the server instance from database pool.
    public ODatabase(string alias)

    // Initiates single dedicated connection with the server instance and retrieves specified database.
    public ODatabase(string hostname, int port, string databaseName, ODatabaseType databaseType, string userName, string userPassword)

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

Reloads clusters and cluster count from server.
    
    public OCluster AddCluster(OClusterType type, string name)

    public OCluster AddCluster(OClusterType type, string name, string location, string dataSegmentName)
    
Adds new cluster to the database.

    public bool RemoveCluster(short clusterID)
    
Removes cluster to the database. Returns boolean indicating if the specified cluster was successfuly removed.

    public int AddSegment(string name, string location)
    
Adds new data segment to the database. Returns ID of newly added data segment.
    
    public bool RemoveSegment(string name)
    
Removes specified data segment from database. Returns boolean indicating if the specified data segment was successfuly removed.
    
    public void Close()
    
Closes connection with server instance and resets session ID assigned to this object. This method is also called when ODatabase instance is being disposed.

    public void Dispose()
    
Closes connection with server instance and disposes ODatabase object.

OCluster class
---

**Constructor**

    public OCluster()
    
Default contructor.

**Properties**

    public short ID { get; set; }
    
Represents ID of the cluster.
    
    public string Name { get; set; }
    
Represents name of the cluster.
    
    public OClusterType Type { get; set; }
    
Represents type of the cluster.
    
    public string Location { get; set; }
    
Represents location of the cluster.
    
    public short DataSegmentID { get; set; }
    
    public string DataSegmentName { get; set; }

    public long RecordsCount { get; }
    
    public long[] DataRange { get; }