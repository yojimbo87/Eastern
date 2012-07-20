using System.Linq;
using Eastern.Connection;

namespace Eastern.Protocol.Operations
{
    internal class CreateDatabase : IOperation
    {
        internal string DatabaseName { get; set; }
        internal ODatabaseType DatabaseType { get; set; }
        internal OStorageType StorageType { get; set; }

        public Request Request(int sessionID)
        {
            Request request = new Connection.Request();
            // standard request fields
            request.DataItems.Add(new DataItem() { Type = "byte", Data = BinaryParser.ToArray((byte)OperationType.DB_CREATE) });
            request.DataItems.Add(new DataItem() { Type = "int", Data = BinaryParser.ToArray(sessionID) });
            // operation specific fields
            request.DataItems.Add(new DataItem() { Type = "string", Data = BinaryParser.ToArray(DatabaseName) });
            request.DataItems.Add(new DataItem() { Type = "string", Data = BinaryParser.ToArray((DatabaseType == ODatabaseType.Document) ? "document" : "graph") });
            request.DataItems.Add(new DataItem() { Type = "string", Data = BinaryParser.ToArray((StorageType == OStorageType.Local) ? "local" : "memory") });

            return request;
        }

        public object Response(Response response)
        {
            // offset not neaded since there are no operation specific response fields
            //int offset = 5;
            bool wasDatabaseCreated = false;

            if (response == null)
            {
                return wasDatabaseCreated;
            }

            if (response.Status == ResponseStatus.OK)
            {
                wasDatabaseCreated = true;
            }

            return wasDatabaseCreated;
        }
    }
}
