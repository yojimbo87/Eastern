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
            WorkerConnection.Close();
            SessionID = -1;
        }
    }
}
