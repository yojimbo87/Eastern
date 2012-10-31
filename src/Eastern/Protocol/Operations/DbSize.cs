using System.Linq;
using Eastern.Connection;

namespace Eastern.Protocol.Operations
{
    internal class DbSize : IOperation
    {
        public Request Request(int sessionID)
        {
            Request request = new Connection.Request();

            // standard request fields
            request.DataItems.Add(new DataItem() { Type = "byte", Data = BinaryParser.ToArray((byte)OperationType.DB_SIZE) });
            request.DataItems.Add(new DataItem() { Type = "int", Data = BinaryParser.ToArray(sessionID) });

            return request;
        }

        public object Response(Response response)
        {
            // start from this position since standard fields (status, session ID) has been already parsed
            int offset = 5;
            long databaseSize = 0;

            if (response == null)
            {
                return databaseSize;
            }

            // operation specific fields
            databaseSize = BinaryParser.ToLong(response.Data.Skip(offset).Take(8).ToArray());
            offset += 8;

            return databaseSize;
        }
    }
}
