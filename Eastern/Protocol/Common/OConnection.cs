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

        public void Close()
        {
            CloseDatabase operation = new CloseDatabase();

            WorkerConnection.ExecuteOperation<CloseDatabase>(operation);
            WorkerConnection.Close();
            SessionID = -1;
        }
    }
}
