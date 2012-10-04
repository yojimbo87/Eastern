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
        private const string _rootPassword = "84079F1F2D9C6DB52DFE94A5F4B0D9F33C2390E70931E16AF77E191B929C857C";
        private const string _databaseName = "test1";
        private const string _username = "admin";
        private const string _password = "admin";

        static void Main(string[] args)
        {
            TestCreateRecord();

            Console.ReadLine();
        }

        static void TestCreateRecord()
        {
            using (ODatabase database = new ODatabase(_hostname, _port, _databaseName, ODatabaseType.Document, _username, _password))
            {
                TestClass foo = new TestClass();
                foo.IsBool = true;
                foo.ByteNumber = 222;
                foo.ShortNumber = 22222;
                foo.IntNumber = 12345678;
                foo.LongNumber = 1234567890123;
                foo.FloatNumber = 3.14f;
                foo.DoubleNumber = 12343.23442;
                foo.DecimalNumber = new Decimal(1234567.890);
                foo.DateTime = DateTime.Now;
                foo.String = "Bra\"vo \\ asdf";

                Console.WriteLine(database.CreateRecord<TestClass>(2, foo, false));
            }
        }

        static void TestParsing()
        {
            string raw;

            // binary
            raw = "single:_AAECAwQFBgcICQoLDA0ODxAREhMUFRYXGBkaGx_,embedded:(binary:_AAECAwQFBgcICQoLDA0ODxAREhMUFRYXGBkaGx_),array:[_AAECAwQFBgcICQoLDA0ODxAREhMUFRYXGBkaGx_,_AAECAwQFBgcICQoLDA0ODxAREhMUFRYXGBkaGx_]";

            // date and datetime
            raw = "datetime:1296279468000t,date:1306281600000a,embedded:(datetime:1296279468000t,date:1306281600000a),array:[1296279468000t,1306281600000a]";

            // boolean
            raw = "singleT:true,singleF:false,embedded:(singleT:true,singleF:false),array:[true,false]";

            // null
            raw = "nick:,embedded:(nick:,joe:),joe:";

            // numbers
            raw = "byte:123b,short:23456s,int:1543345,long:132432455l,float:1234.432f,double:123123.4324d,bigdecimal:12312.24324c,embedded:(byte:123b,short:23456s,int:1543345,long:132432455l,float:1234.432f,double:123123.4324d,bigdecimal:12312.24324c),array:[123b,23456s,1543345,132432455l,1234.432f,123123.4324d,12312.24324c]";

            // map
            raw = "rules:{\"database.query\":2,\"database.command\":2,\"database.hook.record\":2},array:[{\"database.query\":2,\"database.command\":2,\"database.hook.record\":2},{\"database.query\":2,\"database.command\":2,\"database.hook.record\":2}],nested:{\"database.query\":2,\"database.command\":{\"database.query\":2,\"database.command\":2,\"database.hook.record\":2},\"database.hook.record\":2,\"database.hook2.record\":{\"database.hook.record\":2}}";

            // string
            raw = "simple:\"whoa this is awesome\",singleQuoted:\"a" + "\\" + "\"\",doubleQuotes:\"" + "\\" + "\"adsf" + "\\" + "\"\",twoBackslashes:\"" + "\\a" + "\\a" + "\"";

            // array with embedded documents
            raw = "nick:[(joe1:\"js1\"),(joe2:\"js2\"),(joe3:\"s3\")]";

            // complex array and embedded documents
            raw = "mary:[(zak1:(nick:[(joe1:\"js1\"),(joe2:\"js2\"),(joe3:\"s3\")])),(zak2:(nick:[(joe4:\"js4\"),(joe5:\"js5\"),(joe6:\"s6\")]))]";

            // example 1
            raw = "Profile@nick:\"ThePresident\",follows:[],followers:[#10:5,#10:6],name:\"Barack\",surname:\"Obama\",location:#3:2,invitedBy:,salary_cloned:,salary:120.3f";

            // example 2
            raw = "name:\"ORole\",id:0,defaultClusterId:3,clusterIds:[3],properties:[(name:\"mode\",type:17,offset:0,mandatory:false,notNull:false,min:,max:,linkedClass:,linkedType:,index:),(name:\"rules\",type:12,offset:1,mandatory:false,notNull:false,min:,max:,linkedClass:,linkedType:17,index:)]";

            // example 3
            raw = "ORole@name:\"reader\",inheritedRole:,mode:0,rules:{\"database\":2,\"database.cluster.internal\":2,\"database.cluster.orole\":2,\"database.cluster.ouser\":2,\"database.class.*\":2,\"database.cluster.*\":2,\"database.query\":2,\"database.command\":2,\"database.hook.record\":2}";

            Console.WriteLine(raw);
            ORecord record = new ORecord(ORecordType.Document, 0, UTF8Encoding.UTF8.GetBytes(raw));
            PrintDocument(raw, record.ToDocument());
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

    public class TestClass
    {
        public string Null { get; set; }
        public bool IsBool { get; set; }
        public byte ByteNumber { get; set; }
        public short ShortNumber { get; set; }
        public int IntNumber { get; set; }
        public long LongNumber { get; set; }
        public float FloatNumber { get; set; }
        public double DoubleNumber { get; set; }
        public decimal DecimalNumber { get; set; }
        public DateTime DateTime { get; set; }
        public string String { get; set; }
        public string[] StringArray { get; set; }
        public List<string> StringList { get; set; }
        public TestNestedClass NestedClass { get; set; }

        public TestClass()
        {
            StringArray = new string[3];
            StringList = new List<string>();
            NestedClass = new TestNestedClass("nested string xyz");
        }
    }

    public class TestNestedClass
    {
        public string NestedString { get; set; }
        public string[] StringArray { get; set; }
        public List<string> StringList { get; set; }

        public TestNestedClass(string s)
        {
            NestedString = s;
            StringArray = new string[3];
            StringList = new List<string>();
        }
    }
}
