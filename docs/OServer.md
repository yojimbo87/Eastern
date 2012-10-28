Server operations API
---

Server operations are exposed through following public API.

OServer class
---

**Constructor**

    // Initiates single dedicated connection with the server instance.
    public OServer(string hostname, int port, string userName, string userPassword)

**Properties**

    // Represents ID of current session between client and server instance.
    public int SessionID { get; }

    // Indicates if underlying socket is connected to server instance.
    public bool IsConnected { get; }
    
**Methods**

    // Sends shut down command to currently connected server instance.
    // Returns boolean indicating if the server was shut down successfuly.
    public bool Shutdown()

    // Creates new database on currently connected server instance.
    // Returns boolean indicating if the database was created successfuly.
    public bool CreateDatabase(string databaseName, ODatabaseType databaseType, OStorageType storageType)

    // Checks if specified database exists on currently connected server instance.
    // Returns boolean indicating if the database exists.
    public bool DatabaseExist(string databaseName)

    // Deletes specified database on currently connected server instance.
    public void DeleteDatabase(string databaseName)

    // Closes connection with server instance and resets session ID, user name and user password assigned to this object. 
    // This method is also called when ODatabase instance is being disposed.
    public void Close()
    
    // Closes connection with server instance and disposes OServer object.
    public void Dispose()
    
ODatabaseType enum
---

    Document = 0
    Graph = 1
    
OStorageType enum
---

    Remote = 0,
    Local = 1,
    Memory = 2