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

        private Worker WorkerConnection { get; set; }

        public EasternClient()
        {
            WorkerConnection = new Worker();
        }

        public EasternClient(string hostname, int port)
        {
            WorkerConnection = new Worker();
            WorkerConnection.Initialize(hostname, port);
        }

        // return value indicates if the server was shut down successfuly
        public bool Shutdown(string userName, string userPassword)
        {
            Shutdown operation = new Shutdown();
            operation.UserName = userName;
            operation.UserPassword = userPassword;

            return (bool)WorkerConnection.ExecuteOperation<Shutdown>(operation);
        }

        public OConnection Connect(string userName, string userPassword)
        {
            Connect operation = new Connect();
            operation.UserName = userName;
            operation.UserPassword = userPassword;

            OConnection connection = (OConnection)WorkerConnection.ExecuteOperation<Connect>(operation);

            WorkerConnection.SessionID = connection.SessionID;

            connection.WorkerConnection = WorkerConnection;

            return connection;
        }

        public ODatabase OpenDatabase(string databaseName, ODatabaseType databaseType, string userName, string userPassword)
        {
            DbOpen operation = new DbOpen();
            operation.DatabaseName = databaseName;
            operation.DatabaseType = databaseType;
            operation.UserName = userName;
            operation.UserPassword = userPassword;

            ODatabase database = (ODatabase)WorkerConnection.ExecuteOperation<DbOpen>(operation);

            WorkerConnection.SessionID = database.SessionID;

            database.WorkerConnection = WorkerConnection;

            // add worker connection to each cluster
            foreach (OCluster cluster in database.Clusters)
            {
                cluster.WorkerConnection = WorkerConnection;
            }

            return database;
        }

        /*internal static object QueueOperation<T>(T operation)
        {
            return WorkerConnection.ExecuteOperation<T>(operation);
        }*/
    }
}
