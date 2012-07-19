Eastern is a C#/.NET driver for [OrientDB](http://code.google.com/p/orient/) which implements [network binary protocol](http://code.google.com/p/orient/wiki/NetworkBinaryProtocol) to offer the fastest way for communication between .NET based applications and OrientDB server instances.

Status
------

Currently in early stage of development.

Features
--------

- Initial connection with database server
- Request/response handling
- Supported operations:
  - Shutdown (SHUTDOWN)
  - Connect (CONNECT)
  - Open database (DB_OPEN)

TODO
----

- Error/exception handling
- Support entire set of [operations](http://code.google.com/p/orient/wiki/NetworkBinaryProtocol#Operations)
- Test suite
- Support automatic reconnection with retry timer
- Support for client side connection pooling