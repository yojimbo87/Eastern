using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Eastern;

namespace ConsoleTest
{
    class Program
    {
        private static OServer _connection;

        private const string _hostname = "127.0.0.1";
        private const int _port = 2424;
        private const string _rootName = "root";
        private const string _rootPassword = "root";
        private const string _databaseName = "tempEasternUniqueTestDatabase0001x";
        private const string _username = "admin";
        private const string _password = "admin";

        static void Main(string[] args)
        {
            string raw = "Profile@nick:\"ThePr,whoa:esident\",follows:[\"me\",\"you\"],followers:[#10:5,#10:6],name:\"Barack\",surname:\"Obama\",location:#3:2,invitedBy:,salary_cloned:,salary:120.3f";
            ORecord record = new ORecord(ORecordType.Document, 0, UTF8Encoding.UTF8.GetBytes(raw));
            PrintDocument(raw, record.ToDocument());

            raw = "nick:[(nick1:\"xxx\"),(nick2:\"yyy\")],joe:[(joe_1_1:\"xxx\",joe_1_2:\"yyy\")],moe:[(moe_1_1:#3:23,moe_1_2:\",whoa:#1:3,\",moe_1_3:#3:43,moe_1_4:[#124:34433],moe_1_5:[#124:344,#344:23])]";
            record = new ORecord(ORecordType.Document, 0, UTF8Encoding.UTF8.GetBytes(raw));
            PrintDocument(raw, record.ToDocument());

            raw = "nick:";
            record = new ORecord(ORecordType.Document, 0, UTF8Encoding.UTF8.GetBytes(raw));
            PrintDocument(raw, record.ToDocument());

            raw = "nick:,joe:";
            record = new ORecord(ORecordType.Document, 0, UTF8Encoding.UTF8.GetBytes(raw));
            PrintDocument(raw, record.ToDocument());

            raw = "moe:#3:43,joe:\"whoa\",johny:[\"waoh\"],kyle:[\"wwww\",\"\",\"hhhh\"],wise:[#3:13],kate:[#3:554,#55:23]";
            record = new ORecord(ORecordType.Document, 0, UTF8Encoding.UTF8.GetBytes(raw));
            PrintDocument(raw, record.ToDocument());

            raw = "moe:#3:43,joe:\"whoa\",johny:[12],kyle:[13b,45b,244f],huh:12365676t,wow:78910,wise:[5.34f],kate:[6.45f,12.9f]";
            record = new ORecord(ORecordType.Document, 0, UTF8Encoding.UTF8.GetBytes(raw));
            PrintDocument(raw, record.ToDocument());

            Console.ReadLine();
        }

        static void PrintDocument(string raw, ODocument document)
        {
            Console.WriteLine("Raw string: {0}", raw);
            Console.WriteLine("---------------------------------------------");
            Console.WriteLine("Version: {0}, Class name: {1}", document.Version, document.Class);

            foreach (KeyValuePair<string, object> kv in document.Fields)
            {
                if (kv.Value == null)
                {
                    Console.WriteLine("- {0}: null", kv.Key);
                }
                else if (kv.Value.GetType() == typeof(List<String>))
                {
                    Console.Write("- {0} (FC): ", kv.Key);

                    for (int i = 0; i < ((List<String>)kv.Value).Count; i++)
                    //foreach (string value in (List<String>)kv.Value)
                    {
                        Console.Write("{0}", ((List<String>)kv.Value)[i]);

                        if ((i + 1) != ((List<String>)kv.Value).Count)
                        {
                            Console.Write(", ");
                        }
                    }

                    Console.WriteLine();
                }
                else
                {
                    Console.WriteLine("- {0}: {1}", kv.Key, kv.Value);
                }
            }

            Console.WriteLine("=============================================");
        }

        static void Test()
        {
            _connection = new OServer(_hostname, _port, _rootName, _rootPassword);

            if (!_connection.DatabaseExist(_databaseName))
            {
                _connection.CreateDatabase(_databaseName, ODatabaseType.Document, OStorageType.Local);

            }

            try
            {
                using (ODatabase database = new ODatabase(_hostname, _port, _databaseName, ODatabaseType.Document, _username, _password))
                {
                    Console.WriteLine("Session ID: {0}", database.SessionID);

                    ORecord record = database.LoadRecord(4, 0, "*:0", false);
                    ODocument document = record.ToDocument();
                    Console.WriteLine("Version: {0}, Class name: {1}", document.Version, document.Class);
                }
            }
            catch (OException ex)
            {
                Console.WriteLine("{0}: {1}", ex.Type, ex.Description);
            }
            finally
            {
                // delete test database
                _connection.DeleteDatabase(_databaseName);
                _connection.Close();
            }
        }
    }
}
