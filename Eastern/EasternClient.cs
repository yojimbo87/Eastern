using Eastern.Connection;
using Eastern.Protocol.Operations;

namespace Eastern
{
    public class EasternClient
    {
        internal static string DriverName { get { return "Eastern"; } }
        internal static string DriverVersion { get { return "0.0.1 pre-alpha"; } }
        internal static short ProtocolVersion { get { return 12; } }
        internal static string ClientID { get { return "null"; } }

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

        public bool Shutdown(string userName, string userPassword)
        {
            Shutdown operation = new Shutdown();
            operation.UserName = userName;
            operation.UserPassword = userPassword;

            return (bool)Connection.ExecuteOperation<Shutdown>(operation);
        }

        public OConnection Connect(string userName, string userPassword)
        {
            Connect operation = new Connect();
            operation.UserName = userName;
            operation.UserPassword = userPassword;

            return (OConnection)Connection.ExecuteOperation<Connect>(operation);
        }

        public ODatabase OpenDatabase(string databaseName, DatabaseType databaseType, string userName, string userPassword)
        {
            OpenDatabase operation = new OpenDatabase();
            operation.DatabaseName = databaseName;
            operation.DatabaseType = databaseType;
            operation.UserName = userName;
            operation.UserPassword = userPassword;

            return (ODatabase)Connection.ExecuteOperation<OpenDatabase>(operation);
        }
    }
}
