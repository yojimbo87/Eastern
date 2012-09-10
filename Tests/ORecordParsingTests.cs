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

        [TestMethod]
        public void TestNumbers()
        {
            string raw = "byte:123b,short:23456s,int:1543345,long:132432455l,float:1234.432f,double:123123.4324d,bigdecimal:12312.24324c,embedded:(byte:123b,short:23456s,int:1543345,long:132432455l,float:1234.432f,double:123123.4324d,bigdecimal:12312.24324c),array:[123b,23456s,1543345,132432455l,1234.432f,123123.4324d,12312.24324c]";

            ORecord record = new ORecord(ORecordType.Document, 0, UTF8Encoding.UTF8.GetBytes(raw));
            ODocument document = record.ToDocument();

            Assert.IsTrue(document.Fields["byte"].GetType() == typeof(byte));
            Assert.IsTrue((byte)document.Fields["byte"] == 123);

            Assert.IsTrue(document.Fields["short"].GetType() == typeof(short));
            Assert.IsTrue((short)document.Fields["short"] == 23456);

            Assert.IsTrue(document.Fields["int"].GetType() == typeof(int));
            Assert.IsTrue((int)document.Fields["int"] == 1543345);

            Assert.IsTrue(document.Fields["long"].GetType() == typeof(long));
            Assert.IsTrue((long)document.Fields["long"] == 132432455);

            Assert.IsTrue(document.Fields["float"].GetType() == typeof(float));
            Assert.IsTrue((float)document.Fields["float"] == 1234.432f);

            Assert.IsTrue(document.Fields["double"].GetType() == typeof(double));
            Assert.IsTrue((double)document.Fields["double"] == 123123.4324);

            Assert.IsTrue(document.Fields["bigdecimal"].GetType() == typeof(decimal));
            Assert.IsTrue((decimal)document.Fields["bigdecimal"] == new Decimal(12312.24324));

            Dictionary<string, object> embedded = (Dictionary<string, object>)document.Fields["embedded"];

            Assert.IsTrue(embedded["byte"].GetType() == typeof(byte));
            Assert.IsTrue((byte)embedded["byte"] == 123);

            Assert.IsTrue(embedded["short"].GetType() == typeof(short));
            Assert.IsTrue((short)embedded["short"] == 23456);

            Assert.IsTrue(embedded["int"].GetType() == typeof(int));
            Assert.IsTrue((int)embedded["int"] == 1543345);

            Assert.IsTrue(embedded["long"].GetType() == typeof(long));
            Assert.IsTrue((long)embedded["long"] == 132432455);

            Assert.IsTrue(embedded["float"].GetType() == typeof(float));
            Assert.IsTrue((float)embedded["float"] == 1234.432f);

            Assert.IsTrue(embedded["double"].GetType() == typeof(double));
            Assert.IsTrue((double)embedded["double"] == 123123.4324);

            Assert.IsTrue(embedded["bigdecimal"].GetType() == typeof(decimal));
            Assert.IsTrue((decimal)embedded["bigdecimal"] == new Decimal(12312.24324));

            List<object> array = (List<object>)document.Fields["array"];

            Assert.IsTrue(array[0].GetType() == typeof(byte));
            Assert.IsTrue((byte)array[0] == 123);

            Assert.IsTrue(array[1].GetType() == typeof(short));
            Assert.IsTrue((short)array[1] == 23456);

            Assert.IsTrue(array[2].GetType() == typeof(int));
            Assert.IsTrue((int)array[2] == 1543345);

            Assert.IsTrue(array[3].GetType() == typeof(long));
            Assert.IsTrue((long)array[3] == 132432455);

            Assert.IsTrue(array[4].GetType() == typeof(float));
            Assert.IsTrue((float)array[4] == 1234.432f);

            Assert.IsTrue(array[5].GetType() == typeof(double));
            Assert.IsTrue((double)array[5] == 123123.4324);

            Assert.IsTrue(array[6].GetType() == typeof(decimal));
            Assert.IsTrue((decimal)array[6] == new Decimal(12312.24324));
        }

        [TestMethod]
        public void TestMap()
        {
            string raw = "rules:{\"database.query\":2,\"database.command\":2,\"database.hook.record\":2},embedded:(rules:{\"database.query\":2,\"database.command\":2,\"database.hook.record\":2}),array:[{\"database.query\":2,\"database.command\":2,\"database.hook.record\":2},{\"database.query\":2,\"database.command\":2,\"database.hook.record\":2}],nested:{\"database.query\":2,\"database.command\":{\"database.query\":2,\"database.command\":2,\"database.hook.record\":2},\"database.hook.record\":2,\"database.hook2.record\":{\"database.hook.record\":2}}";

            ORecord record = new ORecord(ORecordType.Document, 0, UTF8Encoding.UTF8.GetBytes(raw));
            ODocument document = record.ToDocument();

            Assert.IsTrue(document.Fields["rules"].GetType() == typeof(string));
            Assert.IsTrue((string)document.Fields["rules"] == "{\"database.query\":2,\"database.command\":2,\"database.hook.record\":2}");

            Dictionary<string, object> embedded = (Dictionary<string, object>)document.Fields["embedded"];

            Assert.IsTrue(embedded["rules"].GetType() == typeof(string));
            Assert.IsTrue((string)embedded["rules"] == "{\"database.query\":2,\"database.command\":2,\"database.hook.record\":2}");

            List<object> array = (List<object>)document.Fields["array"];

            Assert.IsTrue(array[0].GetType() == typeof(string));
            Assert.IsTrue((string)array[0] == "{\"database.query\":2,\"database.command\":2,\"database.hook.record\":2}");

            Assert.IsTrue(array[1].GetType() == typeof(string));
            Assert.IsTrue((string)array[1] == "{\"database.query\":2,\"database.command\":2,\"database.hook.record\":2}");

            Assert.IsTrue(document.Fields["nested"].GetType() == typeof(string));
            Assert.IsTrue((string)document.Fields["nested"] == "{\"database.query\":2,\"database.command\":{\"database.query\":2,\"database.command\":2,\"database.hook.record\":2},\"database.hook.record\":2,\"database.hook2.record\":{\"database.hook.record\":2}}");
        }
    }
}
