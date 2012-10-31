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
        private const string _rootPassword = "E651342F2D9C417D56DA4A57BD9D38E7FA991648787837133553B3DE8332457E";
        private const string _databaseName = "test1";
        private const string _username = "admin";
        private const string _password = "admin";

        static void Main(string[] args)
        {
            //TestCreateRecord();
            TestLoadRecord();

            Console.ReadLine();
        }

        static void TestCreateRecord()
        {
            using (ODatabase database = new ODatabase(_hostname, _port, _databaseName, ODatabaseType.Document, _username, _password))
            {
                
            }
        }

        static void TestLoadRecord()
        {
            using (ODatabase database = new ODatabase(_hostname, _port, _databaseName, ODatabaseType.Document, _username, _password))
            {
                TestClass obj = database.LoadRecord<TestClass>(new ORID(6, 0));

                Console.WriteLine(obj);
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

    class TestClass
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
        public List<TestNestedClass> ObjectList { get; set; }
    }

    class TestNestedClass
    {
        public string NestedString { get; set; }
        public string[] StringArray { get; set; }
        public List<string> StringList { get; set; }
    }
}
