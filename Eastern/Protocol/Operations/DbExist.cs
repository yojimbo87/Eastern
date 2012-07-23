using System.Linq;
using Eastern.Connection;

namespace Eastern.Protocol.Operations
{
    internal class DbExist : IOperation
    {
        internal string DatabaseName { get; set; }

        public Request Request(int sessionID)
        {
            Request request = new Connection.Request();

            // standard request fields
            request.DataItems.Add(new DataItem() { Type = "byte", Data = BinaryParser.ToArray((byte)OperationType.DB_EXIST) });
            request.DataItems.Add(new DataItem() { Type = "int", Data = BinaryParser.ToArray(sessionID) });
            // operation specific fields
            request.DataItems.Add(new DataItem() { Type = "string", Data = BinaryParser.ToArray(DatabaseName) });

            return request;
        }

        public object Response(Response response)
        {
            // start from this position since standard fields (status, session ID) has been already parsed
            int offset = 5;
            bool databaseExist = false;

            // operation specific fields
            byte existByte = BinaryParser.ToByte(response.Data.Skip(offset).Take(1).ToArray());
            offset += 1;

            if (existByte > 0)
            {
                databaseExist = true;
            }

            return databaseExist;
        }
    }
}
