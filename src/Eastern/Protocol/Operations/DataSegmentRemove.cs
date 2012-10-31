using System.Linq;
using Eastern.Connection;

namespace Eastern.Protocol.Operations
{
    internal class DataSegmentRemove
    {
        internal string SegmentName { get; set; }

        public Request Request(int sessionID)
        {
            Request request = new Connection.Request();

            // standard request fields
            request.DataItems.Add(new DataItem() { Type = "byte", Data = BinaryParser.ToArray((byte)OperationType.DATASEGMENT_REMOVE) });
            request.DataItems.Add(new DataItem() { Type = "int", Data = BinaryParser.ToArray(sessionID) });

            // operation specific fields
            request.DataItems.Add(new DataItem() { Type = "string", Data = BinaryParser.ToArray(SegmentName) });

            return request;
        }

        public object Response(Response response)
        {
            // start from this position since standard fields (status, session ID) has been already parsed
            int offset = 5;
            bool succeeded = false;

            if (response == null)
            {
                return succeeded;
            }

            // operation specific fields
            succeeded = BinaryParser.ToBoolean(response.Data.Skip(offset).Take(1).ToArray());
            offset += 1;

            return succeeded;
        }
    }
}
