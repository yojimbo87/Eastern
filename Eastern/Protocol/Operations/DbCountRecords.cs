using System.Linq;
using Eastern.Connection;

namespace Eastern.Protocol.Operations
{
    internal class DbCountRecords : IOperation
    {
        public Request Request(int sessionID)
        {
            Request request = new Connection.Request();

            // standard request fields
            request.DataItems.Add(new DataItem() { Type = "byte", Data = BinaryParser.ToArray((byte)OperationType.DB_COUNTRECORDS) });
            request.DataItems.Add(new DataItem() { Type = "int", Data = BinaryParser.ToArray(sessionID) });

            return request;
        }

        public object Response(Response response)
        {
            // start from this position since standard fields (status, session ID) has been already parsed
            int offset = 5;
            long databaseRecordsCount = 0;

            if (response == null)
            {
                return databaseRecordsCount;
            }

            // operation specific fields
            databaseRecordsCount = BinaryParser.ToShort(response.Data.Skip(offset).Take(8).ToArray());
            offset += 8;

            return databaseRecordsCount;
        }
    }
}
