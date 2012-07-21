﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Eastern;

namespace ConsoleTest
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                //TestShutdown();
                //TestConnect();
                //TestOpenDatabase();
                //TestCreateDatabase();
                TestCloseConnection();
                TestCloseDatabase();
            }
            catch (OException ex)
            {
                Console.WriteLine("{0}: {1}", ex.Type, ex.Description);
            }

            Console.ReadLine();
        }

        static void TestShutdown()
        {
            EasternClient client = new EasternClient("127.0.0.1", 2424);
            bool result = client.Shutdown("root", "FFB5AB5CF4F2DC287B83737FCD6F849BB316E2CC952406B5A5DAEC81275A264C");

            Console.WriteLine("Is server down: " + result);

            Console.WriteLine("======================================================");
        }

        static void TestConnect()
        {
            EasternClient client = new EasternClient("127.0.0.1", 2424);
            OConnection connection = client.Connect("root", "FFB5AB5CF4F2DC287B83737FCD6F849BB316E2CC952406B5A5DAEC81275A264C");

            Console.WriteLine("Session ID: " + connection.SessionID);

            Console.WriteLine("======================================================");
        }

        static void TestOpenDatabase()
        {
            EasternClient client = new EasternClient("127.0.0.1", 2424);
            ODatabase db = client.OpenDatabase("test1", ODatabaseType.Document, "admin", "admin");

            Console.WriteLine("Session ID: " + db.SessionID);
            Console.WriteLine("Clusters:");

            foreach (OCluster cluster in db.Clusters)
            {
                Console.WriteLine("    {0} - {1} - {2}", cluster.Name, cluster.Type, cluster.ID);
            }

            Console.WriteLine("======================================================");
        }

        static void TestCreateDatabase()
        {
            EasternClient client = new EasternClient("127.0.0.1", 2424);
            OConnection connection = client.Connect("root", "FFB5AB5CF4F2DC287B83737FCD6F849BB316E2CC952406B5A5DAEC81275A264C");

            Console.WriteLine("Session ID: " + connection.SessionID);

            bool result = connection.CreateDatabase("testCreateDB", ODatabaseType.Document, OStorageType.Local);

            Console.WriteLine("Is database created: " + result);

            Console.WriteLine("======================================================");
        }

        static void TestCloseConnection()
        {
            EasternClient client = new EasternClient("127.0.0.1", 2424);
            OConnection connection = client.Connect("root", "FFB5AB5CF4F2DC287B83737FCD6F849BB316E2CC952406B5A5DAEC81275A264C");

            Console.WriteLine("Session ID: " + connection.SessionID);

            connection.Close();

            Console.WriteLine("Connection closed. Current session ID: " + connection.SessionID);

            Console.WriteLine("======================================================");
        }

        static void TestCloseDatabase()
        {
            EasternClient client = new EasternClient("127.0.0.1", 2424);
            ODatabase database = client.OpenDatabase("test1", ODatabaseType.Document, "admin", "admin");

            Console.WriteLine("Session ID: " + database.SessionID);
            Console.WriteLine("Clusters:");

            foreach (OCluster cluster in database.Clusters)
            {
                Console.WriteLine("    {0} - {1} - {2}", cluster.Name, cluster.Type, cluster.ID);
            }

            database.Close();

            Console.WriteLine("Connection closed. Current session ID: " + database.SessionID);

            Console.WriteLine("======================================================");
        }
    }
}
