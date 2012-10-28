Features
---

Eastern supports following features:

- Connection initiation with remote database server
- Request/response parsing
- Request/response error handling
- Record to POCO and vice versa (de)serialization
- Client side [connection pooling](http://msdn.microsoft.com/en-us/library/8xx3tyca.aspx) of specified database based on [this](http://stackoverflow.com/questions/1148467/is-there-a-standard-way-of-implementing-a-proprietary-connection-pool-in-net) and [this](http://www.codeproject.com/Articles/35011/NET-TCP-Connection-Pooling) example
- Following OrientDB [binary protocol](http://code.google.com/p/orient/wiki/NetworkBinaryProtocol) operations:
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
  - Create record (RECORD_CREATE)
  - Update record (RECORD_UPDATE)
  - Delete record (RECORD_DELETE)
  - Load record (RECORD_LOAD)