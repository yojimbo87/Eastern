using System.Linq;
using Eastern;
using Eastern.Connection;

namespace Eastern.Protocol.Operations
{
    internal class CloseDatabase : IOperation
    {
        public Request Request(int sessionID)
        {
            Request request = new Connection.Request();

            // standard request fields
            request.DataItems.Add(new DataItem() { Type = "byte", Data = BinaryParser.ToArray((byte)OperationType.DB_CLOSE) });
            request.DataItems.Add(new DataItem() { Type = "int", Data = BinaryParser.ToArray(sessionID) });

            request.DataItems.Add(new DataItem() { Type = "byte", Data = BinaryParser.ToArray(0) });

            return request;
        }

        public object Response(Response response)
        {
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
