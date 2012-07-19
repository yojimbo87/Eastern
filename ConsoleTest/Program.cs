using System;
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
            TestShutdown();
            //TestConnect();
            //TestOpenDatabase();

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
            ODatabase db = client.OpenDatabase("test1", DatabaseType.Document, "admin", "admin");

            Console.WriteLine("Session ID: " + db.SessionID);
            Console.WriteLine("Clusters:");

            foreach (Cluster cluster in db.Clusters)
            {
                Console.WriteLine("    {0} - {1} - {2}", cluster.Name, cluster.Type, cluster.ID);
            }

            Console.WriteLine("======================================================");
        }
    }
}
