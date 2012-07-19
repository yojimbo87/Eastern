using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Eastern;

namespace Eastern.Console
{
    class Program
    {
        static void Main(string[] args)
        {
            Eastern eastern = new Eastern();
            System.Console.WriteLine(eastern.Connect("127.0.0.1", 2424));

            Database db = eastern.OpenDatabase("test1", DatabaseType.Document, "admin", "admin");

            System.Console.WriteLine(db.SessionID);

            foreach (Cluster cluster in db.Clusters)
            {
                System.Console.WriteLine("{0} - {1} - {2}", cluster.Name, cluster.Type, cluster.ID);
            }

            System.Console.ReadLine();
        }
    }
}
