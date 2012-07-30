using System.Collections.Generic;

namespace Eastern
{
    internal class DatabasePool
    {
        private string Hostname { get; set; }
        private int Port { get; set; }
        private string DatabaseName { get; set; }
        private ODatabaseType DatabaseType { get; set; }
        private string UserName { get; set; }
        private string UserPassword { get; set; }

        internal string PoolHash
        {
            get
            {
                return Hostname + Port + DatabaseName + DatabaseType.ToString() + UserName;
            }
        }

        internal Queue<ODatabase> Databases { get; set; }

        internal DatabasePool(string hostname, int port, string databaseName, ODatabaseType databaseType, string userName, string userPassword)
        {
            Hostname = hostname;
            Port = port;
            DatabaseName = databaseName;
            DatabaseType = databaseType;
            UserName = userName;
            UserPassword = userPassword;
            Databases = new Queue<ODatabase>();

            for (int i = 0; i < 5; i++)
            {
                ODatabase database = new ODatabase(Hostname, Port, DatabaseName, DatabaseType, UserName, UserPassword);
                database.ReturnToPool = true;

                Databases.Enqueue(database);
            }
        }
    }
}
