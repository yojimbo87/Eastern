using System.Linq;
using Eastern;
using Eastern.Connection;

namespace Eastern.Protocol.Operations
{
    internal class DbClose : IOperation
    {
        public Request Request(int sessionID)
        {
            Request request = new Connection.Request();
            request.OperationMode = OperationMode.Asynchronous;

            // standard request fields
            request.DataItems.Add(new DataItem() { Type = "byte", Data = BinaryParser.ToArray((byte)OperationType.DB_CLOSE) });
            request.DataItems.Add(new DataItem() { Type = "int", Data = BinaryParser.ToArray(sessionID) });

            return request;
        }

        public object Response(Response response)
        {
            // there is no specific response fields processing for this operation

            return null;
        }
    }
}
