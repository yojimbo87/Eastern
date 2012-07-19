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
            EasternClient client = new EasternClient("127.0.0.1", 2424);
            //Console.WriteLine(eastern.Connect("127.0.0.1", 2424));

            Database db = client.OpenDatabase("test1", DatabaseType.Document, "admin", "admin");

            Console.WriteLine(db.SessionID);

            foreach (Cluster cluster in db.Clusters)
            {
                Console.WriteLine("{0} - {1} - {2}", cluster.Name, cluster.Type, cluster.ID);
            }

            Console.ReadLine();
        }
    }
}
