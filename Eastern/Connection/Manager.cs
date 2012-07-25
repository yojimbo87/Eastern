using System.Collections.Generic;

namespace Eastern.Connection
{
    internal class Manager
    {
        private Worker Connection { get; set; }
        private static List<object> Queue { get; set; }

        internal Manager()
        {
            Connection = new Worker();
            Queue = new List<object>();
        }
    }
}
