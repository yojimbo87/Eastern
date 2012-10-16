using System.Linq;
using Eastern.Connection;

namespace Eastern.Protocol.Operations
{
    internal class RecordLoad : IOperation
    {
        internal short ClusterID { get; set; }
        internal long ClusterPosition { get; set; }
        internal string FetchPlan { get; set; }
        internal bool IgnoreCache { get; set; }

        public Request Request(int sessionID)
        {
            Request request = new Connection.Request();

            // standard request fields
            request.DataItems.Add(new DataItem() { Type = "byte", Data = BinaryParser.ToArray((byte)OperationType.RECORD_LOAD) });
            request.DataItems.Add(new DataItem() { Type = "int", Data = BinaryParser.ToArray(sessionID) });
            // operation specific fields
            request.DataItems.Add(new DataItem() { Type = "short", Data = BinaryParser.ToArray(ClusterID) });
            request.DataItems.Add(new DataItem() { Type = "long", Data = BinaryParser.ToArray(ClusterPosition) });
            request.DataItems.Add(new DataItem() { Type = "string", Data = BinaryParser.ToArray(FetchPlan) });
            request.DataItems.Add(new DataItem() { Type = "byte", Data = BinaryParser.ToArray((IgnoreCache == true) ? 1 : 0) });

            return request;
        }

        public object Response(Response response)
        {
            // start from this position since standard fields (status, session ID) has been already parsed
            int offset = 5;
            DtoRecord record = new DtoRecord();

            if (response == null)
            {
                return record;
            }

            // operation specific fields
            PayloadStatus payloadStatus = (PayloadStatus)BinaryParser.ToByte(response.Data.Skip(offset).Take(1).ToArray());
            offset += 1;

            int contentLength = BinaryParser.ToInt(response.Data.Skip(offset).Take(4).ToArray());
            offset += 4;
            record.Content = response.Data.Skip(offset).Take(contentLength).ToArray();
            offset += contentLength;

            record.Version = BinaryParser.ToInt(response.Data.Skip(offset).Take(4).ToArray());
            offset += 4;

            record.Type = (ORecordType)BinaryParser.ToByte(response.Data.Skip(offset).Take(1).ToArray());
            offset += 1;

            return record;
        }
    }
}
