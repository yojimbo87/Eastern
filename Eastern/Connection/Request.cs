using System.Collections.Generic;
using Eastern.Protocol;

namespace Eastern.Connection
{
    internal class Request
    {
        internal OperationMode OperationMode { get; set; }
        internal List<DataItem> DataItems { get; set; }

        internal Request()
        {
            OperationMode = OperationMode.Synchronous;
            DataItems = new List<DataItem>();
        }
    }
}
