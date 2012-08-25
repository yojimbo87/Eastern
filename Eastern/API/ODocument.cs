using System.Collections.Generic;

namespace Eastern
{
    public class ODocument
    {
        public string Class { get; set; }
        public int Version { get; set; }
        public Dictionary<string, object> Fields { get; set; }

        public ODocument()
        {
            Fields = new Dictionary<string, object>();
        }
    }
}
