using System.Collections.Generic;

namespace Eastern.Connection
{
    internal class Request
    {
        internal List<DataItem> DataItems { get; set; }

        internal Request()
        {
            DataItems = new List<DataItem>();
        }
    }
}
