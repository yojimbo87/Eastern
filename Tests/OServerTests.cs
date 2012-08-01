using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Eastern;

namespace Tests
{
    /// <summary>
    /// Summary description for UnitTest1
    /// </summary>
    [TestClass]
    public class OServerTests
    {
        private const string _hostname = "127.0.0.1";
        private const int _port = 2424;
        private const string _rootName = "root";
        private const string _rootPassword = "9F696830A58E8187F6CC36674C666AE73E202DE3B0216C5B8BF8403C276CEE52";
        private const string _databaseName = "test1";
        private const string _username = "admin";
        private const string _password = "admin";

        private const string _newDatabaseName = "testTempNewDatabase001x";

        public OServerTests()
        {
            //
            // TODO: Add constructor logic here
            //
        }

        private TestContext testContextInstance;

        /// <summary>
        ///Gets or sets the test context which provides
        ///information about and functionality for the current test run.
        ///</summary>
        public TestContext TestContext
        {
            get
            {
                return testContextInstance;
            }
            set
            {
                testContextInstance = value;
            }
        }

        #region Additional test attributes
        //
        // You can use the following additional attributes as you write your tests:
        //
        // Use ClassInitialize to run code before running the first test in the class
        // [ClassInitialize()]
        // public static void MyClassInitialize(TestContext testContext) { }
        //
        // Use ClassCleanup to run code after all tests in a class have run
        // [ClassCleanup()]
        // public static void MyClassCleanup() { }
        //
        // Use TestInitialize to run code before running each test 
        // [TestInitialize()]
        // public void MyTestInitialize() { }
        //
        // Use TestCleanup to run code after each test has run
        // [TestCleanup()]
        // public void MyTestCleanup() { }
        //
        #endregion

        [TestMethod]
        public void TestConnect()
        {
            using (OServer connection = new OServer(_hostname, _port, _rootName, _rootPassword))
            {
                bool isGreaterThanZero = (connection.SessionID > 0) ? true : false;

                Assert.IsTrue(isGreaterThanZero);
            }
        }

        [TestMethod]
        public void TestCloseConnection()
        {
            using (OServer connection = new OServer(_hostname, _port, _rootName, _rootPassword))
            {
                connection.Close();

                bool closeResult = (connection.SessionID == -1) ? true : false;

                Assert.IsTrue(closeResult);
            }
        }

        [TestMethod]
        public void TestDatabaseExist()
        {
            using (OServer connection = new OServer(_hostname, _port, _rootName, _rootPassword))
            {
                Assert.IsTrue(connection.DatabaseExist("test1"));
            }
        }

        [TestMethod]
        public void TestDatabaseNotExist()
        {
            using (OServer connection = new OServer(_hostname, _port, _rootName, _rootPassword))
            {
                Assert.IsFalse(connection.DatabaseExist("thisShoulNotExist001x"));
            }
        }

        [TestMethod]
        public void TestDatabaseCreate()
        {
            using (OServer connection = new OServer(_hostname, _port, _rootName, _rootPassword))
            {
                bool databaseCreateResult = connection.CreateDatabase(_newDatabaseName, ODatabaseType.Document, OStorageType.Local);

                Assert.IsTrue(databaseCreateResult);
            }
        }

        [TestMethod]
        public void TestDatabaseDelete()
        {
            using (OServer connection = new OServer(_hostname, _port, _rootName, _rootPassword))
            {
                connection.DeleteDatabase(_newDatabaseName);

                Assert.IsFalse(connection.DatabaseExist(_newDatabaseName));
            }
        }

        /*[TestMethod]
        public void TestServerShutdown()
        {
            using (OServer connection = new OServer(_hostname, _port, _rootName, _rootPassword))
            {
                bool shutdownResult = connection.Shutdown();

                Assert.IsTrue(shutdownResult);
            }
        }*/
    }
}
