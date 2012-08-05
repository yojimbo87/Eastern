using Eastern.Connection;
using Eastern.Protocol.Operations;
using System.Collections.Generic;

namespace Eastern
{
    public static class EasternClient
    {
        private static object SyncRoot { get; set; }
        private static List<ODatabasePool> DatabasePools { get; set; }

        internal static string DriverName { get { return "Eastern"; } }
        internal static string DriverVersion { get { return "0.0.1 pre-alpha"; } }
        internal static short ProtocolVersion { get { return 12; } }
        internal static string ClientID { get { return "null"; } }

        static EasternClient()
        {
            SyncRoot = new object();
            DatabasePools = new List<ODatabasePool>();
        }

        public static void CreateDatabasePool(string hostname, int port, string databaseName, ODatabaseType databaseType, string userName, string userPassword)
        {
            lock (SyncRoot)
            {
                ODatabasePool pool = new ODatabasePool(hostname, port, databaseName, databaseType, userName, userPassword);

                DatabasePools.Add(pool);
            }
        }

        public static ODatabasePool GetDatabasePool(string hostname, int port, string databaseName, ODatabaseType databaseType, string userName)
        {
            lock (SyncRoot)
            {
                string poolHash = hostname + port + databaseName + databaseType.ToString() + userName;

                return DatabasePools.Find(q => q.PoolHash == poolHash);
            }
        }

        public static ODatabase GetDatabase(string hostname, int port, string databaseName, ODatabaseType databaseType, string userName, string userPassword)
        {
            lock (SyncRoot)
            {
                ODatabase database;
                string hash = hostname + port + databaseName + databaseType.ToString() + userName;

                if (DatabasePools.Exists(q => q.PoolHash == hash))
                {
                    ODatabasePool pool = DatabasePools.Find(q => q.PoolHash == hash);

                    if (pool.Databases.Count > 0)
                    {
                        database = pool.Databases.Dequeue();
                        database.Reload();

                        return database;
                    }
                }

                database = new ODatabase(hostname, port, databaseName, databaseType, userName, userPassword);
                database.ReturnToPool = false;

                return database;
            }
        }

        internal static void ReturnDatabase(ODatabase database)
        {
            lock (SyncRoot)
            {
                if (DatabasePools.Exists(q => q.PoolHash == database.Hash) && database.ReturnToPool)
                {
                    ODatabasePool pool = DatabasePools.Find(q => q.PoolHash == database.Hash);

                    pool.Databases.Enqueue(database);
                }
                else
                {
                    database.Close();
                }
            }
        }
    }
}
