using System.Collections.Generic;

namespace Eastern.Connection
{
    internal class Request
    {
        internal bool ExpectResponse { get; set; }
        internal List<DataItem> DataItems { get; set; }

        internal Request()
        {
            ExpectResponse = true;
            DataItems = new List<DataItem>();
        }
    }
}
