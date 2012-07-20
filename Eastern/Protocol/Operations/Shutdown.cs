using System.Linq;
using Eastern;
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
            int offset = 1;
            OConnection connection = new OConnection();

            if (response == null)
            {
                return false;
            }

            // standard response fields
            response.Status = (ResponseStatus)BinaryParser.ToByte(response.Data.Take(1).ToArray());
            response.SessionID = BinaryParser.ToInt(response.Data.Skip(offset).Take(4).ToArray());
            offset += 4;
            // operation specific fields

            if (response.Status == ResponseStatus.OK)
            {
                return true;
            }

            return false;
        }
    }
}
