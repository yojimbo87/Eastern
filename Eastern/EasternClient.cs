using Eastern.Connection;
using Eastern.Protocol.Operations;
using System.Collections.Generic;

namespace Eastern
{
    public static class EasternClient
    {
        private static object SyncRoot { get; set; }
        private static List<DatabasePool> DatabasePools { get; set; }

        internal static string DriverName { get { return "Eastern"; } }
        internal static string DriverVersion { get { return "0.0.1 pre-alpha"; } }
        internal static short ProtocolVersion { get { return 12; } }
        internal static string ClientID { get { return "null"; } }

        static EasternClient()
        {
            SyncRoot = new object();
            DatabasePools = new List<DatabasePool>();
        }

        public static void CreateDatabasePool(string hostname, int port, string databaseName, ODatabaseType databaseType, string userName, string userPassword, int poolSize, string alias)
        {
            lock (SyncRoot)
            {
                DatabasePool pool = new DatabasePool(hostname, port, databaseName, databaseType, userName, userPassword, poolSize, alias);

                DatabasePools.Add(pool);
            }
        }

        public static DatabasePool GetDatabasePool(string poolAlias)
        {
            lock (SyncRoot)
            {
                return DatabasePools.Find(q => q.Alias == poolAlias);
            }
        }

        /*public static int GetDatabasePoolSize(string hostname, int port, string databaseName, ODatabaseType databaseType, string userName)
        {
            lock (SyncRoot)
            {
                string poolHash = hostname + port + databaseName + databaseType.ToString() + userName;
                ODatabasePool pool = DatabasePools.Find(q => q.PoolHash == poolHash);

                return (pool != null) ? pool.PoolSize : 0;
            }
        }*/

        internal static Database GetDatabase(string alias)
        {
            lock (SyncRoot)
            {
                if (DatabasePools.Exists(db => db.Alias == alias))
                {
                    DatabasePool pool = DatabasePools.Find(db => db.Alias == alias);

                    // deque free database connection if the pool has one
                    if (pool.Databases.Count > 0)
                    {
                        return pool.Databases.Dequeue();
                    }
                    // if the pool is empty - create new dedicated database connection
                    else if (pool.Databases.Count == 0)
                    {
                        Database database = new Database(pool.Hostname, pool.Port, pool.DatabaseName, pool.DatabaseType, pool.UserName, pool.UserPassword);
                        database.ReturnToPool = false;

                        return database;
                    }
                }

                return null;
            }
        }

        internal static void ReturnDatabase(Database database)
        {
            lock (SyncRoot)
            {
                if (DatabasePools.Exists(db => db.Alias == database.Alias) && database.ReturnToPool)
                {
                    DatabasePool pool = DatabasePools.Find(q => q.Alias == database.Alias);

                    pool.Databases.Enqueue(database);
                }
                else
                {
                    database.Dispose();
                }
            }
        }
    }
}
