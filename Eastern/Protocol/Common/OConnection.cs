using Eastern.Connection;
using Eastern.Protocol.Operations;

namespace Eastern
{
    public class OConnection
    {
        internal WorkerConnection WorkerConnection { get; set; }

        public int SessionID { get; set; }

        // return value indicates if the database was created successfuly
        public bool CreateDatabase(string databaseName, ODatabaseType databaseType, OStorageType storageType)
        {
            CreateDatabase operation = new CreateDatabase();
            operation.DatabaseName = databaseName;
            operation.DatabaseType = databaseType;
            operation.StorageType = storageType;

            return (bool)WorkerConnection.ExecuteOperation<CreateDatabase>(operation);
        }

        // return value indicates if the database was created successfuly
        public bool Close()
        {
            CloseDatabase operation = new CloseDatabase();

            bool isConnectionClosed = (bool)WorkerConnection.ExecuteOperation<CloseDatabase>(operation);
            
            if (isConnectionClosed)
            {
                WorkerConnection.Close();
            }

            return isConnectionClosed;
        }
    }
}
