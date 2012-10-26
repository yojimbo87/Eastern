﻿using System;
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

            // create test database
            if (!_connection.DatabaseExist(_databaseName))
            {
                _connection.CreateDatabase(_databaseName, ODatabaseType.Document, OStorageType.Local);
            }
        }

        [TestMethod]
        public void TestPocoCreateAndLoadRecord()
        {
            using (ODatabase database = new ODatabase(_hostname, _port, _databaseName, ODatabaseType.Document, _username, _password))
            {
                TestClass foo = new TestClass();
                foo.String = "test string value";

                ORecord recordCreated = database.CreateRecord("TestClass", foo);

                TestClass fooRetrieved = database.LoadRecord<TestClass>(recordCreated.ORID);

                Assert.IsTrue(fooRetrieved.String == foo.String);
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
