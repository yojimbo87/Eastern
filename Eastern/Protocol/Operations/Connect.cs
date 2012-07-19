using System.Collections.Generic;
using System.Linq;
using Eastern.Connection;

namespace Eastern.Protocol.Operations
{
    internal class Connect : BaseOperation, IOperation
    {
        internal string DriverName { get { return "Eastern"; } }
        internal string DriverVersion { get { return "0.0.1 pre-alpha"; } }
        internal short ProtocolVersion { get; set; }
        internal string ClientID { get { return "null"; } }

        internal string UserName { get; set; }
        internal string UserPassword { get; set; }

        internal Connect()
        {
            OperationType = OperationType.CONNECT;
        }

        public Request Request()
        {
            Request request = new Connection.Request();

            // standard request fields
            request.DataItems.Add(new DataItem() { Type = "byte", Data = Parser.ToArray((byte)OperationType) });
            request.DataItems.Add(new DataItem() { Type = "int", Data = Parser.ToArray(SessionID) });
            // operation specific fields
            request.DataItems.Add(new DataItem() { Type = "string", Data = Parser.ToArray(DriverName) });
            request.DataItems.Add(new DataItem() { Type = "string", Data = Parser.ToArray(DriverVersion) });
            request.DataItems.Add(new DataItem() { Type = "short", Data = Parser.ToArray(ProtocolVersion) });
            request.DataItems.Add(new DataItem() { Type = "string", Data = Parser.ToArray(ClientID) });
            request.DataItems.Add(new DataItem() { Type = "string", Data = Parser.ToArray(UserName) });
            request.DataItems.Add(new DataItem() { Type = "string", Data = Parser.ToArray(UserPassword) });

            return request;
        }

        public object Response(Response response)
        {
            int offset = 1;
            OConnection connection = new OConnection();

            // standard response fields
            response.Status = (ResponseStatus)Parser.ToByte(response.Data.Take(1).ToArray());
            response.SessionID = Parser.ToInt(response.Data.Skip(offset).Take(4).ToArray());
            offset += 4;
            // operation specific fields
            connection.SessionID = Parser.ToInt(response.Data.Skip(offset).Take(4).ToArray());
            offset += 4;

            return connection;
        }
    }
}
