using System.Linq;
using Eastern.Connection;

namespace Eastern.Protocol.Operations
{
    internal class DataSegmentAdd
    {
        internal string SegmentName { get; set; }
        internal string SegmentLocation { get; set; }

        public Request Request(int sessionID)
        {
            Request request = new Connection.Request();

            // standard request fields
            request.DataItems.Add(new DataItem() { Type = "byte", Data = BinaryParser.ToArray((byte)OperationType.DATASEGMENT_ADD) });
            request.DataItems.Add(new DataItem() { Type = "int", Data = BinaryParser.ToArray(sessionID) });

            // operation specific fields
            request.DataItems.Add(new DataItem() { Type = "string", Data = BinaryParser.ToArray(SegmentName) });
            request.DataItems.Add(new DataItem() { Type = "string", Data = BinaryParser.ToArray(SegmentLocation) });

            return request;
        }

        public object Response(Response response)
        {
            // start from this position since standard fields (status, session ID) has been already parsed
            int offset = 5;
            int segmentID = 0;

            if (response == null)
            {
                return segmentID;
            }

            // operation specific fields
            segmentID = BinaryParser.ToInt(response.Data.Skip(offset).Take(4).ToArray());
            offset += 4;

            return segmentID;
        }
    }
}
