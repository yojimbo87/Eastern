using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Eastern;

namespace ConsoleTest
{
    class Program
    {
        static string rootPassword = "9F696830A58E8187F6CC36674C666AE73E202DE3B0216C5B8BF8403C276CEE52";
        static string databaseName = "test1";
        static string username = "admin";
        static string password = "admin";

        static void Main(string[] args)
        {
            try
            {
                //TestDbOpen();
                //TestCloseDatabaseConnection();
                //TestDbReload();
                //TestDbSize();
                //TestCountRecords();
                //TestClusterAddClusterRemove();
                //TestClusterCount();
                //TestClusterDataRange();

                //TestShutdown();
            }
            catch (OException ex)
            {
                Console.WriteLine("{0}: {1}", ex.Type, ex.Description);
            }

            Console.ReadLine();
        }

        static void TestDbOpen()
        {
            using (ODatabase database = new ODatabase("127.0.0.1", 2424, databaseName, ODatabaseType.Document, username, password))
            {

                Console.WriteLine("Session ID: " + database.SessionID);
                Console.WriteLine("Clusters:");

                foreach (OCluster cluster in database.Clusters)
                {
                    Console.WriteLine("    {0} - {1} - {2}", cluster.Name, cluster.Type, cluster.ID);
                }
            }

            Console.WriteLine("======================================================");
        }

        static void TestCloseDatabaseConnection()
        {
            using (ODatabase database = new ODatabase("127.0.0.1", 2424, databaseName, ODatabaseType.Document, username, password))
            {
                Console.WriteLine("Session ID: " + database.SessionID);
                Console.WriteLine("Clusters:");

                foreach (OCluster cluster in database.Clusters)
                {
                    Console.WriteLine("    {0} - {1} - {2}", cluster.Name, cluster.Type, cluster.ID);
                }

                database.Close();

                Console.WriteLine("Connection closed. Current session ID: " + database.SessionID);
            }

            Console.WriteLine("======================================================");
        }

        static void TestDbReload()
        {
            using (ODatabase database = new ODatabase("127.0.0.1", 2424, databaseName, ODatabaseType.Document, username, password))
            {
                Console.WriteLine("Session ID: " + database.SessionID);
                Console.WriteLine("Clusters ({0}):", database.ClustersCount);

                foreach (OCluster cluster in database.Clusters)
                {
                    Console.WriteLine("    {0} - {1} - {2}", cluster.Name, cluster.Type, cluster.ID);
                }

                Console.WriteLine("Reloading database...");

                database.Reload();

                Console.WriteLine("Clusters ({0}):", database.ClustersCount);

                foreach (OCluster cluster in database.Clusters)
                {
                    Console.WriteLine("    {0} - {1} - {2}", cluster.Name, cluster.Type, cluster.ID);
                }
            }

            Console.WriteLine("======================================================");
        }

        static void TestDbSize()
        {
            using (ODatabase database = new ODatabase("127.0.0.1", 2424, databaseName, ODatabaseType.Document, username, password))
            {
                Console.WriteLine("Session ID: " + database.SessionID);
                Console.WriteLine("Clusters:");

                foreach (OCluster cluster in database.Clusters)
                {
                    Console.WriteLine("    {0} - {1} - {2}", cluster.Name, cluster.Type, cluster.ID);
                }

                Console.WriteLine("Database size: {0}", database.Size);
            }

            Console.WriteLine("======================================================");
        }

        static void TestCountRecords()
        {
            using (ODatabase database = new ODatabase("127.0.0.1", 2424, databaseName, ODatabaseType.Document, username, password))
            {
                Console.WriteLine("Session ID: " + database.SessionID);
                Console.WriteLine("Clusters:");

                foreach (OCluster cluster in database.Clusters)
                {
                    Console.WriteLine("    {0} - {1} - {2}", cluster.Name, cluster.Type, cluster.ID);
                }

                Console.WriteLine("Database records count: {0}", database.RecordsCount);
            }

            Console.WriteLine("======================================================");
        }

        static void TestClusterAddClusterRemove()
        {
            using (ODatabase database = new ODatabase("127.0.0.1", 2424, databaseName, ODatabaseType.Document, username, password))
            {
                Console.WriteLine("Session ID: " + database.SessionID);
                Console.WriteLine("Clusters:");
                foreach (OCluster cluster in database.Clusters)
                {
                    Console.WriteLine("    {0} - {1} - {2}", cluster.Name, cluster.Type, cluster.ID);
                }

                Console.WriteLine("Adding new cluster...");
                OCluster newCluster = database.AddCluster(OClusterType.Physical, "tempClusterTest1");

                Console.WriteLine("Clusters (before reload):");
                foreach (OCluster cluster in database.Clusters)
                {
                    Console.WriteLine("    {0} - {1} - {2}", cluster.Name, cluster.Type, cluster.ID);
                }

                database.Reload();
                Console.WriteLine("Clusters (after reload):");
                foreach (OCluster cluster in database.Clusters)
                {
                    Console.WriteLine("    {0} - {1} - {2}", cluster.Name, cluster.Type, cluster.ID);
                }

                Console.WriteLine("Removing cluster... {0}", newCluster.ID);
                database.RemoveCluster(newCluster.ID);

                Console.WriteLine("Clusters (before reload):");
                foreach (OCluster cluster in database.Clusters)
                {
                    Console.WriteLine("    {0} - {1} - {2}", cluster.Name, cluster.Type, cluster.ID);
                }

                database.Reload();
                Console.WriteLine("Clusters (after reload):");
                foreach (OCluster cluster in database.Clusters)
                {
                    Console.WriteLine("    {0} - {1} - {2}", cluster.Name, cluster.Type, cluster.ID);
                }
            }

            Console.WriteLine("======================================================");
        }

        static void TestClusterCount()
        {
            using (ODatabase database = new ODatabase("127.0.0.1", 2424, databaseName, ODatabaseType.Document, username, password))
            {
                Console.WriteLine("Session ID: " + database.SessionID);
                Console.WriteLine("Clusters:");
                foreach (OCluster cluster in database.Clusters)
                {
                    Console.WriteLine("    {0} - {1} - {2} - Records count: {3}", cluster.Name, cluster.Type, cluster.ID, cluster.RecordsCount);
                }
            }

            Console.WriteLine("======================================================");
        }

        static void TestClusterDataRange()
        {
            using (ODatabase database = new ODatabase("127.0.0.1", 2424, databaseName, ODatabaseType.Document, username, password))
            {
                Console.WriteLine("Session ID: " + database.SessionID);
                Console.WriteLine("Clusters:");
                foreach (OCluster cluster in database.Clusters)
                {
                    long[] range = cluster.DataRange;
                    Console.WriteLine("    {0} - {1} - {2} - Data range: {3} - {4}", cluster.Name, cluster.Type, cluster.ID, range[0], range[1]);
                }
            }

            Console.WriteLine("======================================================");
        }
    }
}
