using System.Collections.Generic;

namespace Eastern.Connection
{
    internal class Response
    {
        internal ResponseStatus Status { get; set; }
        internal int SessionID { get; set; }
        internal byte[] Data { get; set; }
    }
}
