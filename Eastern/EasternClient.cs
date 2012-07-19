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

        public OConnection Connect(string userName, string userPassword)
        {
            Eastern.Protocol.Operations.Connect operation = new Eastern.Protocol.Operations.Connect();
            operation.SessionID = Connection.SessionID;
            operation.UserName = userName;
            operation.UserPassword = userPassword;

            return (OConnection)Connection.ExecuteOperation<Eastern.Protocol.Operations.Connect>(operation);
        }

        public ODatabase OpenDatabase(string databaseName, DatabaseType databaseType, string userName, string userPassword)
        {
            OpenDatabase operation = new OpenDatabase();
            operation.SessionID = Connection.SessionID;
            operation.ProtocolVersion = Connection.ProtocolVersion;
            operation.DatabaseName = databaseName;
            operation.DatabaseType = databaseType;
            operation.UserName = userName;
            operation.UserPassword = userPassword;

            return (ODatabase)Connection.ExecuteOperation<OpenDatabase>(operation);
        }
    }
}
