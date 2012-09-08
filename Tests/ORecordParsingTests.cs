using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Eastern;

namespace Tests
{
    [TestClass]
    public class ORecordParsingTests
    {
        [TestMethod]
        public void TestBinary()
        {
            string raw = "single:_AAECAwQFBgcICQoLDA0ODxAREhMUFRYXGBkaGx_,embedded:(binary:_AAECAwQFBgcICQoLDA0ODxAREhMUFRYXGBkaGx_),array:[_AAECAwQFBgcICQoLDA0ODxAREhMUFRYXGBkaGx_,_AAECAwQFBgcICQoLDA0ODxAREhMUFRYXGBkaGx_]";

            ORecord record = new ORecord(ORecordType.Document, 0, UTF8Encoding.UTF8.GetBytes(raw));
            ODocument document = record.ToDocument();

            Assert.IsTrue(document.Fields["single"].GetType() == typeof(byte[]));
            Assert.IsTrue(((Dictionary<string, object>)document.Fields["embedded"])["binary"].GetType() == typeof(byte[]));

            foreach (object item in (List<object>)document.Fields["array"])
            {
                Assert.IsTrue(item.GetType() == typeof(byte[]));
            }
        }

        [TestMethod]
        public void TestDateTime()
        {
            string raw = "datetime:1296279468000t,date:1306281600000a,embedded:(datetime:1296279468000t,date:1306281600000a),array:[1296279468000t,1306281600000a]";

            ORecord record = new ORecord(ORecordType.Document, 0, UTF8Encoding.UTF8.GetBytes(raw));
            ODocument document = record.ToDocument();

            Assert.IsTrue(document.Fields["datetime"].GetType() == typeof(DateTime));
            Assert.IsTrue((DateTime)document.Fields["datetime"] == new DateTime(2011, 1, 29, 5, 37, 48));

            Assert.IsTrue(document.Fields["date"].GetType() == typeof(DateTime));
            Assert.IsTrue((DateTime)document.Fields["date"] == new DateTime(2011, 5, 25, 0, 0, 0));

            Dictionary<string, object> embeddedDates = (Dictionary<string, object>)document.Fields["embedded"];

            Assert.IsTrue(embeddedDates["datetime"].GetType() == typeof(DateTime));
            Assert.IsTrue((DateTime)embeddedDates["datetime"] == new DateTime(2011, 1, 29, 5, 37, 48));

            Assert.IsTrue(embeddedDates["date"].GetType() == typeof(DateTime));
            Assert.IsTrue((DateTime)embeddedDates["date"] == new DateTime(2011, 5, 25, 0, 0, 0));

            List<object> arrayDates = (List<object>)document.Fields["array"];

            Assert.IsTrue(arrayDates.First().GetType() == typeof(DateTime));
            Assert.IsTrue((DateTime)arrayDates.First() == new DateTime(2011, 1, 29, 5, 37, 48));

            Assert.IsTrue(arrayDates.Last().GetType() == typeof(DateTime));
            Assert.IsTrue((DateTime)arrayDates.Last() == new DateTime(2011, 5, 25, 0, 0, 0));
        }

        [TestMethod]
        public void TestBoolean()
        {
            string raw = "singleT:true,singleF:false,embedded:(singleT:true,singleF:false),array:[true,false]";

            ORecord record = new ORecord(ORecordType.Document, 0, UTF8Encoding.UTF8.GetBytes(raw));
            ODocument document = record.ToDocument();

            Assert.IsTrue(document.Fields["singleT"].GetType() == typeof(bool));
            Assert.IsTrue((bool)document.Fields["singleT"] == true);

            Assert.IsTrue(document.Fields["singleF"].GetType() == typeof(bool));
            Assert.IsTrue((bool)document.Fields["singleF"] == false);

            Dictionary<string, object> embedded = (Dictionary<string, object>)document.Fields["embedded"];

            Assert.IsTrue(embedded["singleT"].GetType() == typeof(bool));
            Assert.IsTrue((bool)embedded["singleT"] == true);

            Assert.IsTrue(embedded["singleF"].GetType() == typeof(bool));
            Assert.IsTrue((bool)embedded["singleF"] == false);

            List<object> array = (List<object>)document.Fields["array"];

            Assert.IsTrue(array.First().GetType() == typeof(bool));
            Assert.IsTrue((bool)array.First() == true);

            Assert.IsTrue(array.Last().GetType() == typeof(bool));
            Assert.IsTrue((bool)array.Last() == false);
        }

        [TestMethod]
        public void TestNull()
        {
            string raw = "nick:,embedded:(nick:,joe:),joe:";

            ORecord record = new ORecord(ORecordType.Document, 0, UTF8Encoding.UTF8.GetBytes(raw));
            ODocument document = record.ToDocument();

            Assert.IsTrue(document.Fields["nick"] == null);

            Dictionary<string, object> embedded = (Dictionary<string, object>)document.Fields["embedded"];

            Assert.IsTrue(embedded["nick"] == null);
            Assert.IsTrue(embedded["joe"] == null);

            Assert.IsTrue(document.Fields["joe"] == null);
        }
    }
}
