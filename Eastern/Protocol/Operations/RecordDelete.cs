using System.Linq;
using Eastern.Connection;

namespace Eastern.Protocol.Operations
{
    internal class RecordDelete : IOperation
    {
        internal short ClusterID { get; set; }
        internal long ClusterPosition { get; set; }
        internal int RecordVersion { get; set; }
        internal OperationMode OperationMode { get; set; }

        public Request Request(int sessionID)
        {
            Request request = new Connection.Request();

            // standard request fields
            request.DataItems.Add(new DataItem() { Type = "byte", Data = BinaryParser.ToArray((byte)OperationType.RECORD_LOAD) });
            request.DataItems.Add(new DataItem() { Type = "int", Data = BinaryParser.ToArray(sessionID) });
            // operation specific fields
            request.DataItems.Add(new DataItem() { Type = "short", Data = BinaryParser.ToArray(ClusterID) });
            request.DataItems.Add(new DataItem() { Type = "long", Data = BinaryParser.ToArray(ClusterPosition) });
            request.DataItems.Add(new DataItem() { Type = "int", Data = BinaryParser.ToArray(RecordVersion) });
            request.DataItems.Add(new DataItem() { Type = "byte", Data = BinaryParser.ToArray((byte)OperationMode) });

            return request;
        }

        public object Response(Response response)
        {
            // start from this position since standard fields (status, session ID) has been already parsed
            int offset = 5;

            bool deleteResult = false;

            if (response == null)
            {
                return deleteResult;
            }

            // operation specific fields
            deleteResult = BinaryParser.ToByte(response.Data.Skip(offset).Take(1).ToArray()) == 1 ? true : false;
            offset += 1;

            return deleteResult;
        }
    }
}
