using Eastern.Connection;
using Eastern.Protocol.Operations;

namespace Eastern
{
    public class EasternClient
    {
        private WorkerConnection Connection { get; set; }

        public EasternClient()
        {
            Connection = new WorkerConnection();
        }

        public EasternClient(string hostname, int port)
        {
            Connection = new WorkerConnection();

            Connection.Initialize(hostname, port);
        }

        public short Connect(string hostname, int port)
        {
            return Connection.Initialize(hostname, port);
        }

        // TODO: add necessary parameters and pass them along the way
        public Database OpenDatabase(string databaseName, DatabaseType databaseType, string userName, string userPassword)
        {
            OpenDatabase operation = new OpenDatabase();
            operation.SessionID = Connection.SessionID;
            operation.ProtocolVersion = Connection.ProtocolVersion;
            operation.DatabaseName = databaseName;
            operation.DatabaseType = databaseType;
            operation.UserName = userName;
            operation.UserPassword = userPassword;

            return (Database)Connection.ExecuteOperation<OpenDatabase>(operation);
        }
    }
}
