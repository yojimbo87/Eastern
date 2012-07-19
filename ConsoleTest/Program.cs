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
            EasternDriver eastern = new EasternDriver();
            Console.WriteLine(eastern.Connect("127.0.0.1", 2424));

            Database db = eastern.OpenDatabase("test1", DatabaseType.Document, "admin", "admin");

            Console.WriteLine(db.SessionID);

            foreach (Cluster cluster in db.Clusters)
            {
                Console.WriteLine("{0} - {1} - {2}", cluster.Name, cluster.Type, cluster.ID);
            }

            Console.ReadLine();
        }
    }
}
