using System.Linq;
using Eastern;
using Eastern.Connection;

namespace Eastern.Protocol.Operations
{
    internal class Connect : IOperation
    {
        internal string UserName { get; set; }
        internal string UserPassword { get; set; }

        public Request Request(int sessionID)
        {
            Request request = new Connection.Request();

            // standard request fields
            request.DataItems.Add(new DataItem() { Type = "byte", Data = BinaryParser.ToArray((byte)OperationType.CONNECT) });
            request.DataItems.Add(new DataItem() { Type = "int", Data = BinaryParser.ToArray(sessionID) });
            // operation specific fields
            request.DataItems.Add(new DataItem() { Type = "string", Data = BinaryParser.ToArray(EasternClient.DriverName) });
            request.DataItems.Add(new DataItem() { Type = "string", Data = BinaryParser.ToArray(EasternClient.DriverVersion) });
            request.DataItems.Add(new DataItem() { Type = "short", Data = BinaryParser.ToArray(EasternClient.ProtocolVersion) });
            request.DataItems.Add(new DataItem() { Type = "string", Data = BinaryParser.ToArray(EasternClient.ClientID) });
            request.DataItems.Add(new DataItem() { Type = "string", Data = BinaryParser.ToArray(UserName) });
            request.DataItems.Add(new DataItem() { Type = "string", Data = BinaryParser.ToArray(UserPassword) });

            return request;
        }

        public object Response(Response response)
        {
            // start from this position since standard fields (status, session ID) has been already parsed
            int offset = 5;
            OConnection connection = new OConnection();

            // operation specific fields
            connection.SessionID = BinaryParser.ToInt(response.Data.Skip(offset).Take(4).ToArray());
            offset += 4;

            return connection;
        }
    }
}
