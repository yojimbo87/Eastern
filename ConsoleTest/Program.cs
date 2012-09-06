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
            string raw = "Profile@nick:\"ThePr,whoa:esident\",follows:(moe:#1:1,joe:#1:3),followers:(moe:\"whoa:,\"),name:\"Barack\",surname:\"Obama\",location:#3:2,invitedBy:,salary_cloned:";
            ORecord record = new ORecord(ORecordType.Document, 0, UTF8Encoding.UTF8.GetBytes(raw));
            PrintDocument(raw, record.ToDocument());

            raw = "nick:";
            record = new ORecord(ORecordType.Document, 0, UTF8Encoding.UTF8.GetBytes(raw));
            PrintDocument(raw, record.ToDocument());

            raw = "nick:,joe:";
            record = new ORecord(ORecordType.Document, 0, UTF8Encoding.UTF8.GetBytes(raw));
            PrintDocument(raw, record.ToDocument());

            raw = "nick:[\"s1\",\"s2\",\"s3\"]";
            record = new ORecord(ORecordType.Document, 0, UTF8Encoding.UTF8.GetBytes(raw));
            PrintDocument(raw, record.ToDocument());

            raw = "nick:[(joe1:\"js1\"),(joe2:\"js2\"),(joe3:\"s3\")]";
            record = new ORecord(ORecordType.Document, 0, UTF8Encoding.UTF8.GetBytes(raw));
            PrintDocument(raw, record.ToDocument());

            raw = "mary:(nick:[(joe1:\"js1\"),(joe2:\"js2\"),(joe3:\"s3\")])";
            record = new ORecord(ORecordType.Document, 0, UTF8Encoding.UTF8.GetBytes(raw));
            PrintDocument(raw, record.ToDocument());

            raw = "mary:[(zak1:(nick:[(joe1:\"js1\"),(joe2:\"js2\"),(joe3:\"s3\")])),(zak2:(nick:[(joe4:\"js4\"),(joe5:\"js5\"),(joe6:\"s6\")]))]";
            record = new ORecord(ORecordType.Document, 0, UTF8Encoding.UTF8.GetBytes(raw));
            PrintDocument(raw, record.ToDocument());

            raw = "moe:#3:43,joe:\"whoa\",johny:[\"waoh\"],kyle:[\"wwww\",\"\",\"hhhh\"],wise:[#3:13],kate:[#3:554,#55:23]";
            record = new ORecord(ORecordType.Document, 0, UTF8Encoding.UTF8.GetBytes(raw));
            PrintDocument(raw, record.ToDocument());

            raw = "nick:[(nick1:\"xxx\"),(nick2:\"yyy\")],joe:[(joe_1_1:\"xxx\",joe_1_2:\"yyy\")],moe:[(moe_1_1:#3:23,moe_1_2:\",whoa:#1:3,\",moe_1_3:#3:43,moe_1_4:[#124:34433],moe_1_5:[#124:344,#344:23])]";
            record = new ORecord(ORecordType.Document, 0, UTF8Encoding.UTF8.GetBytes(raw));
            PrintDocument(raw, record.ToDocument());

            /*raw = "moe:#3:43,joe:\"whoa\",johny:[12],kyle:[13b,45b,244f],huh:12365676t,wow:78910,wise:[5.34f],kate:[6.45f,12.9f]";
            record = new ORecord(ORecordType.Document, 0, UTF8Encoding.UTF8.GetBytes(raw));
            PrintDocument(raw, record.ToDocument());

            raw = "kyle:[(kyle1:13b),(kyle2:45b),(kyle3:244f)],joe:[(joe1_1:[1,2,4],joe1_2:12b),(joe2_1:3443.334,joe2_2:[\"asd\",\"fds\"])],hoe:(goe:(shmoe:4))";
            record = new ORecord(ORecordType.Document, 0, UTF8Encoding.UTF8.GetBytes(raw));
            PrintDocument(raw, record.ToDocument());*/

            Console.ReadLine();
        }

        static void PrintDocument(string raw, ODocument document)
        {
            Console.WriteLine("Raw string: {0}", raw);
            Console.WriteLine("---------------------------------------------");
            Console.WriteLine("Version: {0}, Class name: {1}", document.Version, document.Class);
            Console.WriteLine("---------------------------------------------");

            PrintTree(0, document.Fields);

            Console.WriteLine("=============================================");
        }

        static void PrintTree(int level, Dictionary<string, object> properties)
        {
            foreach (KeyValuePair<string, object> kve in properties)
            {
                if (kve.Value == null)
                {
                    for (int i = 0; i < level; i++)
                    {
                        Console.Write(" ");
                    }
                    Console.WriteLine("- {0}: null", kve.Key);
                }
                else if (kve.Value.GetType() == typeof(List<object>))
                {
                    for (int i = 0; i < level; i++)
                    {
                        Console.Write(" ");
                    }
                    Console.Write("- {0} (COL): ", kve.Key);
                    bool isNewLined = false;
                    int index = 1;

                    foreach(object item in (List<object>)kve.Value)
                    {
                        if (item.GetType() == typeof(Dictionary<string, object>))
                        {
                            isNewLined = true;
                            Console.WriteLine();
                            PrintTree(level + 2, (Dictionary<string, object>)item);
                        }
                        else
                        {
                            Console.Write("{0}", item);

                            if (index != ((List<object>)kve.Value).Count)
                            {
                                Console.Write(", ");
                            }

                            index++;
                        }
                    }

                    if (!isNewLined)
                    {
                        Console.WriteLine();
                    }
                }
                else if (kve.Value.GetType() == typeof(Dictionary<string, object>))
                {
                    for (int i = 0; i < level; i++)
                    {
                        Console.Write(" ");
                    }
                    Console.Write("- {0} (EMB): ", kve.Key);
                    Console.WriteLine();
                    PrintTree(level + 2, (Dictionary<string, object>)kve.Value);
                }
                else
                {
                    for (int i = 0; i < level; i++)
                    {
                        Console.Write(" ");
                    }
                    Console.WriteLine("- {0}: {1}", kve.Key, kve.Value);
                }
            }
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
