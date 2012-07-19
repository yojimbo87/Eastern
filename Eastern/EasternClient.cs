using Eastern.Connection;
using Eastern.Protocol.Operations;

namespace Eastern
{
    public class EasternClient
    {
        private WorkerConnection Client { get; set; }

        public EasternClient()
        {
            Client = new WorkerConnection();
        }

        public EasternClient(string hostname, int port)
        {
            Client = new WorkerConnection();

            Client.Initialize(hostname, port);
        }

        public short Connect(string hostname, int port)
        {
            return Client.Initialize(hostname, port);
        }

        // TODO: add necessary parameters and pass them along the way
        public Database OpenDatabase(string databaseName, DatabaseType databaseType, string userName, string userPassword)
        {
            OpenDatabase operation = new OpenDatabase();
            operation.SessionID = Client.SessionID;
            operation.ProtocolVersion = Client.ProtocolVersion;
            operation.DatabaseName = databaseName;
            operation.DatabaseType = databaseType;
            operation.UserName = userName;
            operation.UserPassword = userPassword;

            return (Database)Client.ExecuteOperation<OpenDatabase>(operation);
        }
    }
}
