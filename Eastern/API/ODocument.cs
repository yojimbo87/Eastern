using System.Threading;
using Eastern.Connection;

namespace Eastern
{
    public class ODocument
    {
        private Database _database { get; set; }

        public ODocument()
        {
            _database = (Database)Thread.GetData(Thread.GetNamedDataSlot(EasternClient.ThreadLocalDatabaseSlotName));
        }

        public string GetDatabaseName()
        {
            return _database.Name;
        }
    }
}
