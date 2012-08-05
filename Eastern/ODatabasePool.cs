using System.Collections.Generic;

namespace Eastern
{
    public class ODatabasePool
    {
        private string Hostname { get; set; }
        private int Port { get; set; }
        private string DatabaseName { get; set; }
        private ODatabaseType DatabaseType { get; set; }
        private string UserName { get; set; }
        private string UserPassword { get; set; }


        public int CurrentPoolSize
        {
            get 
            {
                return Databases.Count;
            }
        }

        public int PoolSize
        {
            get;
            private set;
        }

        public string PoolHash
        {
            get
            {
                return Hostname + Port + DatabaseName + DatabaseType.ToString() + UserName;
            }
        }

        public Queue<ODatabase> Databases { get; set; }

        public ODatabasePool(string hostname, int port, string databaseName, ODatabaseType databaseType, string userName, string userPassword, int poolSize)
        {
            Hostname = hostname;
            Port = port;
            DatabaseName = databaseName;
            DatabaseType = databaseType;
            UserName = userName;
            UserPassword = userPassword;
            PoolSize = poolSize;
            Databases = new Queue<ODatabase>();

            for (int i = 0; i < poolSize; i++)
            {
                ODatabase database = new ODatabase(Hostname, Port, DatabaseName, DatabaseType, UserName, UserPassword);
                database.ReturnToPool = true;

                Databases.Enqueue(database);
            }
        }
    }
}
