using System.Linq;
using Eastern.Connection;

namespace Eastern.Protocol.Operations
{
    internal class RecordCreate : IOperation
    {
        internal int SegmentID { get; set; }
        internal short ClusterID { get; set; }
        internal byte[] RecordContent { get; set; }
        internal ORecordType RecordType { get; set; }
        internal OperationMode OperationMode { get; set; }

        public Request Request(int sessionID)
        {
            Request request = new Connection.Request();

            // standard request fields
            request.DataItems.Add(new DataItem() { Type = "byte", Data = BinaryParser.ToArray((byte)OperationType.RECORD_LOAD) });
            request.DataItems.Add(new DataItem() { Type = "int", Data = BinaryParser.ToArray(sessionID) });
            // operation specific fields
            request.DataItems.Add(new DataItem() { Type = "int", Data = BinaryParser.ToArray(SegmentID) });
            request.DataItems.Add(new DataItem() { Type = "short", Data = BinaryParser.ToArray(ClusterID) });
            request.DataItems.Add(new DataItem() { Type = "bytes", Data = RecordContent });
            request.DataItems.Add(new DataItem() { Type = "byte", Data = BinaryParser.ToArray((byte)RecordType) });
            request.DataItems.Add(new DataItem() { Type = "byte", Data = BinaryParser.ToArray((byte)OperationMode) });

            return request;
        }

        public object Response(Response response)
        {
            // start from this position since standard fields (status, session ID) has been already parsed
            int offset = 5;
            Record record = new Record();
            record.ORID.ClusterID = ClusterID;

            if (response == null)
            {
                return record;
            }

            // operation specific fields
            record.ORID.ClusterPosition = BinaryParser.ToLong(response.Data.Skip(offset).Take(8).ToArray());
            offset += 8;

            record.Version = BinaryParser.ToInt(response.Data.Skip(offset).Take(4).ToArray());
            offset += 4;

            return record;
        }
    }
}
