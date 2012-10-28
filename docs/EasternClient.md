Connection pooling and globals API
---

Connection pooling and globals are exposed through following public API.

EasternClient class (static)
---

**Properties**

    // Represents name of the driver.
    public static string DriverName { get; }
    
    // Represents version of the driver.
    public static string DriverVersion { get; }
    
    // Represents protocol version which this driver supports.
    public static short ProtocolVersion { get; }

**Methods**

    // Creates pool of pre-initiated database connections to the specified server instance. 
    // Alias parameter determines name of the pool.
    public static void CreateDatabasePool(
        string hostname, 
        int port, 
        string databaseName, 
        ODatabaseType databaseType, 
        string userName, 
        string userPassword, 
        int poolSize, 
        string alias
    )

ODatabaseType enum
---

    Document = 0
    Graph = 1