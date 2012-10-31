using System.Linq;
using Eastern.Connection;

namespace Eastern.Protocol.Operations
{
    internal class Shutdown : IOperation
    {
        internal string UserName { get; set; }
        internal string UserPassword { get; set; }

        public Request Request(int sessionID)
        {
            Request request = new Connection.Request();

            // standard request fields
            request.DataItems.Add(new DataItem() { Type = "byte", Data = BinaryParser.ToArray((byte)OperationType.SHUTDOWN) });
            request.DataItems.Add(new DataItem() { Type = "int", Data = BinaryParser.ToArray(sessionID) });
            // operation specific fields
            request.DataItems.Add(new DataItem() { Type = "string", Data = BinaryParser.ToArray(UserName) });
            request.DataItems.Add(new DataItem() { Type = "string", Data = BinaryParser.ToArray(UserPassword) });

            return request;
        }

        public object Response(Response response)
        {
            bool wasShutdownSuccessful = false;

            if (response == null)
            {
                return wasShutdownSuccessful;
            }

            if (response.Status == ResponseStatus.OK)
            {
                wasShutdownSuccessful = true;
            }

            return wasShutdownSuccessful;
        }
    }
}
