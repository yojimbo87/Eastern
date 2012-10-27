using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using System.Configuration;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Eastern;

namespace Tests
{
    [TestClass]
    public class ODatabaseCrudTests : IDisposable
    {
        private OServer _connection;

        private string _hostname = ConfigurationManager.AppSettings["hostname"];
        private int _port = int.Parse(ConfigurationManager.AppSettings["port"]);
        private string _rootName = ConfigurationManager.AppSettings["root.name"];
        private string _rootPassword = ConfigurationManager.AppSettings["root.password"];
        private string _databaseName = ConfigurationManager.AppSettings["database.name"];
        private string _username = ConfigurationManager.AppSettings["user.name"];
        private string _password = ConfigurationManager.AppSettings["user.password"];

        public ODatabaseCrudTests()
        {
            _connection = new OServer(_hostname, _port, _rootName, _rootPassword);

            // if database exists, delete it
            if (_connection.DatabaseExist(_databaseName))
            {
                _connection.DeleteDatabase(_databaseName);
            }

            _connection.CreateDatabase(_databaseName, ODatabaseType.Document, OStorageType.Local);
        }

        [TestMethod]
        public void TestPocoCreateAndLoadRecord()
        {
            using (ODatabase database = new ODatabase(_hostname, _port, _databaseName, ODatabaseType.Document, _username, _password))
            {
                TestClass foo = new TestClass();

                // basic types
                foo.IsBool = true;
                foo.ByteNumber = 22;
                foo.ShortNumber = 22222;
                foo.IntNumber = 12345678;
                foo.LongNumber = 1234567890123;
                foo.FloatNumber = 3.14f;
                foo.DoubleNumber = 12343.23442;
                foo.DecimalNumber = new Decimal(1234567.890);
                foo.DateTime = DateTime.Now;
                foo.String = "Bra\"vo \\ asdf";
                
                // collections
                foo.StringArray = new string[] { "str elem 1", "str elem 2", "str elem 3" };
                foo.StringList = new List<string>() { "str list elem 1", "str list elem 2", "str list elem 3" };
                
                // embedded document
                foo.NestedClass = new TestNestedClass();
                foo.NestedClass.NestedString = "test nested string";
                foo.NestedClass.StringArray = new string[] { "str elem 1", "str elem 2", "str elem 3" };
                foo.NestedClass.StringList = new List<string>() { "str list elem 1", "str list elem 2", "str list elem 3" };

                // collection of embedded documents
                foo.ObjectList = new List<TestNestedClass>();
                foo.ObjectList.Add(new TestNestedClass() { 
                    NestedString = "test embedded doc 1 string",
                    StringArray = new string[] { "str elem 1", "str elem 2", "str elem 3" },
                    StringList = new List<string>() { "str list elem 1", "str list elem 2", "str list elem 3" }
                });
                foo.ObjectList.Add(new TestNestedClass()
                {
                    NestedString = "test embedded doc2 string",
                    StringArray = new string[] { "str elem 11", "str elem 12", "str elem 13" },
                    StringList = new List<string>() { "str list elem 11", "str list elem 12", "str list elem 13" }
                });

                ORecord recordCreated = database.CreateRecord("TestClass", foo);

                TestClass fooRetrieved = database.LoadRecord<TestClass>(recordCreated.ORID);

                // basic types
                Assert.IsTrue(fooRetrieved.Null == null);
                Assert.IsTrue(fooRetrieved.IsBool == foo.IsBool);
                Assert.IsTrue(fooRetrieved.ByteNumber == foo.ByteNumber);
                Assert.IsTrue(fooRetrieved.ByteNumber == foo.ByteNumber);
                Assert.IsTrue(fooRetrieved.IntNumber == foo.IntNumber);
                Assert.IsTrue(fooRetrieved.LongNumber == foo.LongNumber);
                Assert.IsTrue(fooRetrieved.FloatNumber == foo.FloatNumber);
                Assert.IsTrue(fooRetrieved.DoubleNumber == foo.DoubleNumber);
                Assert.IsTrue(fooRetrieved.DecimalNumber == foo.DecimalNumber);
                Assert.IsTrue(fooRetrieved.DateTime.ToString() == foo.DateTime.ToString());
                Assert.IsTrue(fooRetrieved.String == foo.String);

                // collections
                Assert.IsTrue(fooRetrieved.StringArray.Length == foo.StringArray.Length);
                Assert.IsTrue(fooRetrieved.StringArray[0] == foo.StringArray[0]);
                Assert.IsTrue(fooRetrieved.StringArray[1] == foo.StringArray[1]);
                Assert.IsTrue(fooRetrieved.StringArray[2] == foo.StringArray[2]);
                Assert.IsTrue(fooRetrieved.StringList.Count == foo.StringList.Count);
                Assert.IsTrue(fooRetrieved.StringList[0] == foo.StringList[0]);
                Assert.IsTrue(fooRetrieved.StringList[1] == foo.StringList[1]);
                Assert.IsTrue(fooRetrieved.StringList[2] == foo.StringList[2]);

                // embedded document
                Assert.IsTrue(fooRetrieved.NestedClass.NestedString == foo.NestedClass.NestedString);
                Assert.IsTrue(fooRetrieved.NestedClass.StringArray.Length == foo.NestedClass.StringArray.Length);
                Assert.IsTrue(fooRetrieved.NestedClass.StringArray[0] == foo.NestedClass.StringArray[0]);
                Assert.IsTrue(fooRetrieved.NestedClass.StringArray[1] == foo.NestedClass.StringArray[1]);
                Assert.IsTrue(fooRetrieved.NestedClass.StringArray[2] == foo.NestedClass.StringArray[2]);
                Assert.IsTrue(fooRetrieved.NestedClass.StringList.Count == foo.NestedClass.StringList.Count);
                Assert.IsTrue(fooRetrieved.NestedClass.StringList[0] == foo.NestedClass.StringList[0]);
                Assert.IsTrue(fooRetrieved.NestedClass.StringList[1] == foo.NestedClass.StringList[1]);
                Assert.IsTrue(fooRetrieved.NestedClass.StringList[2] == foo.NestedClass.StringList[2]);

                // collection of embedded documents
                Assert.IsTrue(fooRetrieved.ObjectList.Count == foo.ObjectList.Count);
                Assert.IsTrue(fooRetrieved.ObjectList[0].NestedString == foo.ObjectList[0].NestedString);
                Assert.IsTrue(fooRetrieved.ObjectList[0].StringArray.Length == foo.ObjectList[0].StringArray.Length);
                Assert.IsTrue(fooRetrieved.ObjectList[0].StringArray[0] == foo.ObjectList[0].StringArray[0]);
                Assert.IsTrue(fooRetrieved.ObjectList[0].StringArray[1] == foo.ObjectList[0].StringArray[1]);
                Assert.IsTrue(fooRetrieved.ObjectList[0].StringArray[2] == foo.ObjectList[0].StringArray[2]);
                Assert.IsTrue(fooRetrieved.ObjectList[0].StringList.Count == foo.ObjectList[0].StringList.Count);
                Assert.IsTrue(fooRetrieved.ObjectList[0].StringList[0] == foo.ObjectList[0].StringList[0]);
                Assert.IsTrue(fooRetrieved.ObjectList[0].StringList[1] == foo.ObjectList[0].StringList[1]);
                Assert.IsTrue(fooRetrieved.ObjectList[0].StringList[2] == foo.ObjectList[0].StringList[2]);
                Assert.IsTrue(fooRetrieved.ObjectList[1].NestedString == foo.ObjectList[1].NestedString);
                Assert.IsTrue(fooRetrieved.ObjectList[1].StringArray.Length == foo.ObjectList[1].StringArray.Length);
                Assert.IsTrue(fooRetrieved.ObjectList[1].StringArray[0] == foo.ObjectList[1].StringArray[0]);
                Assert.IsTrue(fooRetrieved.ObjectList[1].StringArray[1] == foo.ObjectList[1].StringArray[1]);
                Assert.IsTrue(fooRetrieved.ObjectList[1].StringArray[2] == foo.ObjectList[1].StringArray[2]);
                Assert.IsTrue(fooRetrieved.ObjectList[1].StringList.Count == foo.ObjectList[1].StringList.Count);
                Assert.IsTrue(fooRetrieved.ObjectList[1].StringList[0] == foo.ObjectList[1].StringList[0]);
                Assert.IsTrue(fooRetrieved.ObjectList[1].StringList[1] == foo.ObjectList[1].StringList[1]);
                Assert.IsTrue(fooRetrieved.ObjectList[1].StringList[2] == foo.ObjectList[1].StringList[2]);
            }
        }

        [TestMethod]
        public void TestPocoCreateAndUpdateAndLoadRecord()
        {
            using (ODatabase database = new ODatabase(_hostname, _port, _databaseName, ODatabaseType.Document, _username, _password))
            {
                TestClass foo = new TestClass();

                // basic types
                foo.IsBool = true;
                foo.ByteNumber = 22;
                foo.ShortNumber = 22222;
                foo.IntNumber = 12345678;
                foo.LongNumber = 1234567890123;
                foo.FloatNumber = 3.14f;
                foo.DoubleNumber = 12343.23442;
                foo.DecimalNumber = new Decimal(1234567.890);
                foo.DateTime = DateTime.Now;
                foo.String = "Bra\"vo \\ asdf";

                // collections
                foo.StringArray = new string[] { "str elem 1", "str elem 2", "str elem 3" };
                foo.StringList = new List<string>() { "str list elem 1", "str list elem 2", "str list elem 3" };

                // embedded document
                foo.NestedClass = new TestNestedClass();
                foo.NestedClass.NestedString = "test nested string";
                foo.NestedClass.StringArray = new string[] { "str elem 1", "str elem 2", "str elem 3" };
                foo.NestedClass.StringList = new List<string>() { "str list elem 1", "str list elem 2", "str list elem 3" };

                // collection of embedded documents
                foo.ObjectList = new List<TestNestedClass>();
                foo.ObjectList.Add(new TestNestedClass()
                {
                    NestedString = "test embedded doc 1 string",
                    StringArray = new string[] { "str elem 1", "str elem 2", "str elem 3" },
                    StringList = new List<string>() { "str list elem 1", "str list elem 2", "str list elem 3" }
                });
                foo.ObjectList.Add(new TestNestedClass()
                {
                    NestedString = "test embedded doc2 string",
                    StringArray = new string[] { "str elem 11", "str elem 12", "str elem 13" },
                    StringList = new List<string>() { "str list elem 11", "str list elem 12", "str list elem 13" }
                });

                ORecord recordCreated = database.CreateRecord("TestClass", foo);
                TestClass fooRetrieved = database.LoadRecord<TestClass>(recordCreated.ORID);
                string newStringValue = "new value of string after update";
                fooRetrieved.String = newStringValue;
                foo.String = newStringValue;
                int newVersion = database.UpdateRecord(recordCreated.ORID, fooRetrieved);

                Assert.IsTrue(newVersion == (recordCreated.Version + 1));

                fooRetrieved = null;
                fooRetrieved = database.LoadRecord<TestClass>(recordCreated.ORID);

                // basic types
                Assert.IsTrue(fooRetrieved.Null == null);
                Assert.IsTrue(fooRetrieved.IsBool == foo.IsBool);
                Assert.IsTrue(fooRetrieved.ByteNumber == foo.ByteNumber);
                Assert.IsTrue(fooRetrieved.ByteNumber == foo.ByteNumber);
                Assert.IsTrue(fooRetrieved.IntNumber == foo.IntNumber);
                Assert.IsTrue(fooRetrieved.LongNumber == foo.LongNumber);
                Assert.IsTrue(fooRetrieved.FloatNumber == foo.FloatNumber);
                Assert.IsTrue(fooRetrieved.DoubleNumber == foo.DoubleNumber);
                Assert.IsTrue(fooRetrieved.DecimalNumber == foo.DecimalNumber);
                Assert.IsTrue(fooRetrieved.DateTime.ToString() == foo.DateTime.ToString());
                Assert.IsTrue(fooRetrieved.String == foo.String);

                // collections
                Assert.IsTrue(fooRetrieved.StringArray.Length == foo.StringArray.Length);
                Assert.IsTrue(fooRetrieved.StringArray[0] == foo.StringArray[0]);
                Assert.IsTrue(fooRetrieved.StringArray[1] == foo.StringArray[1]);
                Assert.IsTrue(fooRetrieved.StringArray[2] == foo.StringArray[2]);
                Assert.IsTrue(fooRetrieved.StringList.Count == foo.StringList.Count);
                Assert.IsTrue(fooRetrieved.StringList[0] == foo.StringList[0]);
                Assert.IsTrue(fooRetrieved.StringList[1] == foo.StringList[1]);
                Assert.IsTrue(fooRetrieved.StringList[2] == foo.StringList[2]);

                // embedded document
                Assert.IsTrue(fooRetrieved.NestedClass.NestedString == foo.NestedClass.NestedString);
                Assert.IsTrue(fooRetrieved.NestedClass.StringArray.Length == foo.NestedClass.StringArray.Length);
                Assert.IsTrue(fooRetrieved.NestedClass.StringArray[0] == foo.NestedClass.StringArray[0]);
                Assert.IsTrue(fooRetrieved.NestedClass.StringArray[1] == foo.NestedClass.StringArray[1]);
                Assert.IsTrue(fooRetrieved.NestedClass.StringArray[2] == foo.NestedClass.StringArray[2]);
                Assert.IsTrue(fooRetrieved.NestedClass.StringList.Count == foo.NestedClass.StringList.Count);
                Assert.IsTrue(fooRetrieved.NestedClass.StringList[0] == foo.NestedClass.StringList[0]);
                Assert.IsTrue(fooRetrieved.NestedClass.StringList[1] == foo.NestedClass.StringList[1]);
                Assert.IsTrue(fooRetrieved.NestedClass.StringList[2] == foo.NestedClass.StringList[2]);

                // collection of embedded documents
                Assert.IsTrue(fooRetrieved.ObjectList.Count == foo.ObjectList.Count);
                Assert.IsTrue(fooRetrieved.ObjectList[0].NestedString == foo.ObjectList[0].NestedString);
                Assert.IsTrue(fooRetrieved.ObjectList[0].StringArray.Length == foo.ObjectList[0].StringArray.Length);
                Assert.IsTrue(fooRetrieved.ObjectList[0].StringArray[0] == foo.ObjectList[0].StringArray[0]);
                Assert.IsTrue(fooRetrieved.ObjectList[0].StringArray[1] == foo.ObjectList[0].StringArray[1]);
                Assert.IsTrue(fooRetrieved.ObjectList[0].StringArray[2] == foo.ObjectList[0].StringArray[2]);
                Assert.IsTrue(fooRetrieved.ObjectList[0].StringList.Count == foo.ObjectList[0].StringList.Count);
                Assert.IsTrue(fooRetrieved.ObjectList[0].StringList[0] == foo.ObjectList[0].StringList[0]);
                Assert.IsTrue(fooRetrieved.ObjectList[0].StringList[1] == foo.ObjectList[0].StringList[1]);
                Assert.IsTrue(fooRetrieved.ObjectList[0].StringList[2] == foo.ObjectList[0].StringList[2]);
                Assert.IsTrue(fooRetrieved.ObjectList[1].NestedString == foo.ObjectList[1].NestedString);
                Assert.IsTrue(fooRetrieved.ObjectList[1].StringArray.Length == foo.ObjectList[1].StringArray.Length);
                Assert.IsTrue(fooRetrieved.ObjectList[1].StringArray[0] == foo.ObjectList[1].StringArray[0]);
                Assert.IsTrue(fooRetrieved.ObjectList[1].StringArray[1] == foo.ObjectList[1].StringArray[1]);
                Assert.IsTrue(fooRetrieved.ObjectList[1].StringArray[2] == foo.ObjectList[1].StringArray[2]);
                Assert.IsTrue(fooRetrieved.ObjectList[1].StringList.Count == foo.ObjectList[1].StringList.Count);
                Assert.IsTrue(fooRetrieved.ObjectList[1].StringList[0] == foo.ObjectList[1].StringList[0]);
                Assert.IsTrue(fooRetrieved.ObjectList[1].StringList[1] == foo.ObjectList[1].StringList[1]);
                Assert.IsTrue(fooRetrieved.ObjectList[1].StringList[2] == foo.ObjectList[1].StringList[2]);
            }
        }

        [TestMethod]
        public void TestPocoCreateAndLoadAndDeleteRecord()
        {
            using (ODatabase database = new ODatabase(_hostname, _port, _databaseName, ODatabaseType.Document, _username, _password))
            {
                TestClass foo = new TestClass();
                foo.IsBool = true;

                ORecord recordCreated = database.CreateRecord("TestClass", foo);
                TestClass fooRetrieved = database.LoadRecord<TestClass>(recordCreated.ORID);

                Assert.IsTrue(fooRetrieved.IsBool == foo.IsBool);

                bool isRecordDeleted = database.DeleteRecord(recordCreated.ORID);

                Assert.IsTrue(isRecordDeleted);

                fooRetrieved = null;
                fooRetrieved = database.LoadRecord<TestClass>(recordCreated.ORID);

                Assert.IsTrue(fooRetrieved == null);
            }
        }

        public void Dispose()
        {
            // delete test database
            if (_connection.DatabaseExist(_databaseName))
            {
                _connection.DeleteDatabase(_databaseName);
            }

            _connection.Close();
        }
    }
}
