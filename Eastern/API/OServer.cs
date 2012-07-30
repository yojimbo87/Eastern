using System;
using Eastern.Connection;
using Eastern.Protocol.Operations;

namespace Eastern
{
    public class OServer : IDisposable
    {
        private Worker WorkerConnection { get; set; }
        private string UserName { get; set; }
        private string UserPassword { get; set; }

        public int SessionID { get { return WorkerConnection.SessionID; } }

        public OServer(string hostname, int port, string userName, string userPassword)
        {
            WorkerConnection = new Worker();
            WorkerConnection.Initialize(hostname, port);
            UserName = userName;
            UserPassword = userPassword;

            Connect operation = new Connect();
            operation.UserName = UserName;
            operation.UserPassword = UserPassword;

            WorkerConnection.SessionID = (int)WorkerConnection.ExecuteOperation<Connect>(operation);
        }

        // return value indicates if the server was shut down successfuly
        public bool Shutdown()
        {
            Shutdown operation = new Shutdown();
            operation.UserName = UserName;
            operation.UserPassword = UserPassword;

            return (bool)WorkerConnection.ExecuteOperation<Shutdown>(operation);
        }

        // return value indicates if the database was created successfuly
        public bool CreateDatabase(string databaseName, ODatabaseType databaseType, OStorageType storageType)
        {
            DbCreate operation = new DbCreate();
            operation.DatabaseName = databaseName;
            operation.DatabaseType = databaseType;
            operation.StorageType = storageType;

            return (bool)WorkerConnection.ExecuteOperation<DbCreate>(operation);
        }

        // return value indicates if the database exists
        public bool DatabaseExist(string databaseName)
        {
            DbExist operation = new DbExist();
            operation.DatabaseName = databaseName;

            return (bool)WorkerConnection.ExecuteOperation<DbExist>(operation);
        }

        public void DeleteDatabase(string databaseName)
        {
            DbDelete operation = new DbDelete();
            operation.DatabaseName = databaseName;

            WorkerConnection.ExecuteOperation<DbDelete>(operation);
        }

        public void Close()
        {
            DbClose operation = new DbClose();

            WorkerConnection.ExecuteOperation<DbClose>(operation);
            WorkerConnection.SessionID = -1;
            WorkerConnection.Close();
            WorkerConnection = null;

            UserName = "";
            UserPassword = "";
        }

        public void Dispose()
        {
            Close();
        }
    }
}
