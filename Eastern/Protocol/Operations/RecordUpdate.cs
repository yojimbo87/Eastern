using System.Linq;
using Eastern.Connection;

namespace Eastern.Protocol.Operations
{
    internal class RecordUpdate : IOperation
    {
        internal short ClusterID { get; set; }
        internal long ClusterPosition { get; set; }
        internal byte[] RecordContent { get; set; }
        internal int RecordVersion { get; set; }
        internal ORecordType RecordType { get; set; }
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
            request.DataItems.Add(new DataItem() { Type = "bytes", Data = RecordContent });
            request.DataItems.Add(new DataItem() { Type = "int", Data = BinaryParser.ToArray(RecordVersion) });
            request.DataItems.Add(new DataItem() { Type = "byte", Data = BinaryParser.ToArray((byte)RecordType) });
            request.DataItems.Add(new DataItem() { Type = "byte", Data = BinaryParser.ToArray((byte)OperationMode) });

            return request;
        }

        public object Response(Response response)
        {
            // start from this position since standard fields (status, session ID) has been already parsed
            int offset = 5;

            int recordVersion = 0;

            if (response == null)
            {
                return recordVersion;
            }

            // operation specific fields
            recordVersion = BinaryParser.ToInt(response.Data.Skip(offset).Take(4).ToArray());
            offset += 4;

            return recordVersion;
        }
    }
}
