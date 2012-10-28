Connecting to server and database
---

There are two ways how Eastern can connect to remote OrientDB instance depending on what set of operations you want to perform.

Server connection
---

Server connection is used to perform server specific (or maintenance) operations like create or delete database. List of operations supported by server connection can be found in [OServer API](https://github.com/yojimbo87/Eastern/blob/master/docs/api/OServer.md).

    using (OServer connection = new OServer("127.0.0.1", 2424, "root", "yourRootPwd"))
    {
        connection.CreateDatabase(_databaseName, ODatabaseType.Document, OStorageType.Local);

        bool databaseExist = connection.DatabaseExist(_databaseName));

        if (databaseExist)
        {
            connection.DeleteDatabase(_databaseName);
        }
    }
    
Database connection
---

Database connection is used to perform specific database and data manipulation operations. Connection can be initiated in dedicated way or through pre-initiated connection pool. List of operations supported by database connection can be found in [ODatabase API](https://github.com/yojimbo87/Eastern/blob/master/docs/api/ODatabase.md).

**Dedicated connection**

    using (ODatabase database = new ODatabase("127.0.0.1", 2424, "yourDatabaseName", ODatabaseType.Document, "admin", "admin"))
    {
        OCluster newCluster = database.AddCluster(OClusterType.Physical, "yourNewCluster");
    }
    
**Pooled connection**

    // created database pool with 20 pre-initiated connection to specified database
    EasternClient.CreateDatabasePool("127.0.0.1", 2424, "yourDatabaseName", ODatabaseType.Document, "admin", "admin", 20, "yourDatabasePool");
    
    using (ODatabase database = new ODatabase("yourDatabasePool"))
    {
        OCluster newCluster = database.AddCluster(OClusterType.Physical, "yourNewCluster");
    }
    
Use of connection pooling can speedup application because connection to database server are pre-initiated and stored within a pool which handles retrieving of ready to use database connections on demand.