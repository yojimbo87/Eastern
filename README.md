Eastern is a C#/.NET driver for [OrientDB](http://code.google.com/p/orient/) which implements [network binary protocol](http://code.google.com/p/orient/wiki/NetworkBinaryProtocol) to offer the fastest way for communication between .NET based applications and OrientDB server instances.

Status
---

Alpha 1 (unstable):
- Basic server and database operations are implemented 
- CRUD record operations with POCO (de)serialization is implemented (except special use of LINKSET types)

Installation
---

- Clone a repository and build Eastern project which gives you dll that you can reference

Usage
---

- soon

API
---

- [Database operations API](https://github.com/yojimbo87/Eastern/blob/master/docs/ODatabase.md)
- [Server operations API](https://github.com/yojimbo87/Eastern/blob/master/docs/OServer.md)
- Connection pooling and globals API - soon

Under the hood
---

- [List of implemented features](https://github.com/yojimbo87/Eastern/blob/master/docs/Features.md)
- How Eastern works - soon

License
---

MIT [licensed](https://github.com/yojimbo87/Eastern/blob/master/LICENSE.md).
    
EasternClient class
---

**Methods**

    public static void CreateDatabasePool(string hostname, int port, string databaseName, ODatabaseType databaseType, string userName, string userPassword, int poolSize)
    
    public static ODatabase GetDatabase(string hostname, int port, string databaseName, ODatabaseType databaseType, string userName, string userPassword)

TODO
---

- Support entire set of [operations](http://code.google.com/p/orient/wiki/NetworkBinaryProtocol#Operations)
- Automatic reconnection of pooled connection with retry timer
- Implement [special use of LINKSET types](http://code.google.com/p/orient/wiki/NetworkBinaryProtocol#Special_use_of_LINKSET_types)