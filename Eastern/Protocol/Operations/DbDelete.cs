using System.Linq;
using Eastern.Connection;

namespace Eastern.Protocol.Operations
{
    internal class DbDelete : IOperation
    {
        internal string DatabaseName { get; set; }

        public Request Request(int sessionID)
        {
            Request request = new Connection.Request();
            // standard request fields
            request.DataItems.Add(new DataItem() { Type = "byte", Data = BinaryParser.ToArray((byte)OperationType.DB_DELETE) });
            request.DataItems.Add(new DataItem() { Type = "int", Data = BinaryParser.ToArray(sessionID) });
            // operation specific fields
            request.DataItems.Add(new DataItem() { Type = "string", Data = BinaryParser.ToArray(DatabaseName) });

            return request;
        }

        public object Response(Response response)
        {
            // there is no specific response fields processing for this operation

            return null;
        }
    }
}
