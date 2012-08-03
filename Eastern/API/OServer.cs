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

        /// <summary>
        /// Represents ID of current session between client and server instance.
        /// </summary>
        public int SessionID { get { return WorkerConnection.SessionID; } }

        /// <summary>
        /// Initiates single dedicated connection with the server instance.
        /// </summary>
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

        /// <summary>
        /// Sends shut down command to currently connected server instance.
        /// </summary>
        /// <returns>
        /// Boolean indicating if the server was shut down successfuly.
        /// </returns>
        public bool Shutdown()
        {
            Shutdown operation = new Shutdown();
            operation.UserName = UserName;
            operation.UserPassword = UserPassword;

            return (bool)WorkerConnection.ExecuteOperation<Shutdown>(operation);
        }

        /// <summary>
        /// Creates new database on currently connected server instance.
        /// </summary>
        /// <returns>
        /// Boolean indicating if the database was created successfuly.
        /// </returns>
        public bool CreateDatabase(string databaseName, ODatabaseType databaseType, OStorageType storageType)
        {
            DbCreate operation = new DbCreate();
            operation.DatabaseName = databaseName;
            operation.DatabaseType = databaseType;
            operation.StorageType = storageType;

            return (bool)WorkerConnection.ExecuteOperation<DbCreate>(operation);
        }

        /// <summary>
        /// Checks if specified database exists on currently connected server instance.
        /// </summary>
        /// <returns>
        /// Boolean indicating if the database exists.
        /// </returns>
        public bool DatabaseExist(string databaseName)
        {
            DbExist operation = new DbExist();
            operation.DatabaseName = databaseName;

            return (bool)WorkerConnection.ExecuteOperation<DbExist>(operation);
        }

        /// <summary>
        /// Deletes specified database on currently connected server instance.
        /// </summary>
        public void DeleteDatabase(string databaseName)
        {
            DbDelete operation = new DbDelete();
            operation.DatabaseName = databaseName;

            WorkerConnection.ExecuteOperation<DbDelete>(operation);
        }

        /// <summary>
        /// Closes connection with server instance and resets session ID, user name and user password for current object.
        /// </summary>
        public void Close()
        {
            DbClose operation = new DbClose();

            WorkerConnection.ExecuteOperation<DbClose>(operation);
            WorkerConnection.SessionID = -1;
            WorkerConnection.Close();

            UserName = "";
            UserPassword = "";
        }

        /// <summary>
        /// Closes connection with server instance and disposes current object.
        /// </summary>
        public void Dispose()
        {
            Close();
        }
    }
}
