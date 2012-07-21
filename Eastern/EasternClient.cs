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

        // return value indicates if the server was shut down successfuly
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

            OConnection connection = (OConnection)Connection.ExecuteOperation<Connect>(operation);

            Connection.SessionID = connection.SessionID;

            connection.WorkerConnection = Connection;

            return connection;
        }

        public ODatabase OpenDatabase(string databaseName, ODatabaseType databaseType, string userName, string userPassword)
        {
            DbOpen operation = new DbOpen();
            operation.DatabaseName = databaseName;
            operation.DatabaseType = databaseType;
            operation.UserName = userName;
            operation.UserPassword = userPassword;

            ODatabase database = (ODatabase)Connection.ExecuteOperation<DbOpen>(operation);

            Connection.SessionID = database.SessionID;

            database.WorkerConnection = Connection;

            return database;
        }
    }
}
