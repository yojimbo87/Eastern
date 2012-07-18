using Eastern.Connection;
using Eastern.Protocol.Operations;

namespace Eastern
{
    public class Eastern
    {
        private WorkerClient Client { get; set; }

        public Eastern()
        {
            Client = new WorkerClient();
        }

        public short Connect(string hostname, int port)
        {
            return Client.Connect(hostname, port);
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
