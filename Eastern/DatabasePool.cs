using System.Collections.Generic;
using Eastern.Connection;

namespace Eastern
{
    public class DatabasePool
    {
        internal string Hostname { get; set; }
        internal int Port { get; set; }
        internal string DatabaseName { get; set; }
        internal ODatabaseType DatabaseType { get; set; }
        internal string UserName { get; set; }
        internal string UserPassword { get; set; }
        internal Queue<Database> Databases { get; set; }

        public int CurrentPoolSize { get { return Databases.Count; } }
        public int PoolSize { get; private set; }
        public string Alias { get; set; }

        public DatabasePool(string hostname, int port, string databaseName, ODatabaseType databaseType, string userName, string userPassword, int poolSize, string alias)
        {
            Hostname = hostname;
            Port = port;
            DatabaseName = databaseName;
            DatabaseType = databaseType;
            UserName = userName;
            UserPassword = userPassword;
            PoolSize = poolSize;
            Alias = alias;
            Databases = new Queue<Database>();

            for (int i = 0; i < poolSize; i++)
            {
                Database database = new Database(Hostname, Port, DatabaseName, DatabaseType, UserName, UserPassword);
                database.Alias = alias;
                database.ReturnToPool = true;

                Databases.Enqueue(database);
            }
        }
    }
}
