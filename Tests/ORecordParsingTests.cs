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

        [TestMethod]
        public void TestString()
        {
            string raw = "simple:\"whoa this is awesome\",singleQuoted:\"a" + "\\" + "\"\",doubleQuotes:\"" + "\\" + "\"adsf" + "\\" + "\"\",twoBackslashes:\"" + "\\a" + "\\a" + "\"";

            ORecord record = new ORecord(ORecordType.Document, 0, UTF8Encoding.UTF8.GetBytes(raw));
            ODocument document = record.ToDocument();

            Assert.IsTrue(document.Fields["simple"].GetType() == typeof(string));
            Assert.IsTrue((string)document.Fields["simple"] == "whoa this is awesome");

            Assert.IsTrue(document.Fields["singleQuoted"].GetType() == typeof(string));
            Assert.IsTrue((string)document.Fields["singleQuoted"] == "a\"");

            Assert.IsTrue(document.Fields["doubleQuotes"].GetType() == typeof(string));
            Assert.IsTrue((string)document.Fields["doubleQuotes"] == "\"adsf\"");

            Assert.IsTrue(document.Fields["twoBackslashes"].GetType() == typeof(string));
            Assert.IsTrue((string)document.Fields["twoBackslashes"] == "\\a\\a");
        }

        [TestMethod]
        public void TestEmbeddedDocumentsArray()
        {
            string raw = "nick:[(joe1:\"js1\"),(joe2:\"js2\"),(joe3:\"s3\")]";

            ORecord record = new ORecord(ORecordType.Document, 0, UTF8Encoding.UTF8.GetBytes(raw));
            ODocument document = record.ToDocument();

            List<object> array = (List<object>)document.Fields["nick"];

            Dictionary<string, object> embedded = (Dictionary<string, object>)array[0];

            Assert.IsTrue(embedded["joe1"].GetType() == typeof(string));
            Assert.IsTrue((string)embedded["joe1"] == "js1");

            embedded = (Dictionary<string, object>)array[1];

            Assert.IsTrue(embedded["joe2"].GetType() == typeof(string));
            Assert.IsTrue((string)embedded["joe2"] == "js2");

            embedded = (Dictionary<string, object>)array[2];

            Assert.IsTrue(embedded["joe3"].GetType() == typeof(string));
            Assert.IsTrue((string)embedded["joe3"] == "s3");
        }

        [TestMethod]
        public void TestComplexEmbeddedDocumentsArray()
        {
            string raw = "mary:[(zak1:(nick:[(joe1:\"js1\"),(joe2:\"js2\"),(joe3:\"s3\")])),(zak2:(nick:[(joe4:\"js4\"),(joe5:\"js5\"),(joe6:\"s6\")]))]";

            ORecord record = new ORecord(ORecordType.Document, 0, UTF8Encoding.UTF8.GetBytes(raw));
            ODocument document = record.ToDocument();

            // mary
            List<object> array1 = (List<object>)document.Fields["mary"];
            // zak1
            Dictionary<string, object> embedded1 = (Dictionary<string, object>)array1[0];
            // nick
            Dictionary<string, object> embedded2 = (Dictionary<string, object>)embedded1["zak1"];

            List<object> array2 = (List<object>)embedded2["nick"];
            Dictionary<string, object> embedded3 = (Dictionary<string, object>)array2[0];

            Assert.IsTrue(embedded3["joe1"].GetType() == typeof(string));
            Assert.IsTrue((string)embedded3["joe1"] == "js1");

            embedded3 = (Dictionary<string, object>)array2[1];

            Assert.IsTrue(embedded3["joe2"].GetType() == typeof(string));
            Assert.IsTrue((string)embedded3["joe2"] == "js2");

            embedded3 = (Dictionary<string, object>)array2[2];

            Assert.IsTrue(embedded3["joe3"].GetType() == typeof(string));
            Assert.IsTrue((string)embedded3["joe3"] == "s3");

            // zak2
            embedded1 = (Dictionary<string, object>)array1[1];
            // nick
            embedded2 = (Dictionary<string, object>)embedded1["zak2"];

            // joe1
            array2 = (List<object>)embedded2["nick"];
            embedded3 = (Dictionary<string, object>)array2[0];

            Assert.IsTrue(embedded3["joe4"].GetType() == typeof(string));
            Assert.IsTrue((string)embedded3["joe4"] == "js4");

            embedded3 = (Dictionary<string, object>)array2[1];

            Assert.IsTrue(embedded3["joe5"].GetType() == typeof(string));
            Assert.IsTrue((string)embedded3["joe5"] == "js5");

            embedded3 = (Dictionary<string, object>)array2[2];

            Assert.IsTrue(embedded3["joe6"].GetType() == typeof(string));
            Assert.IsTrue((string)embedded3["joe6"] == "s6");
        }

        [TestMethod]
        public void TestExample1()
        {
            string raw = "Profile@nick:\"ThePresident\",follows:[],followers:[#10:5,#10:6],name:\"Barack\",surname:\"Obama\",location:#3:2,invitedBy:,salary_cloned:,salary:120.3f";

            ORecord record = new ORecord(ORecordType.Document, 0, UTF8Encoding.UTF8.GetBytes(raw));
            ODocument document = record.ToDocument();

            Assert.IsTrue(document.Class == "Profile");

            Assert.IsTrue(document.Fields["nick"].GetType() == typeof(string));
            Assert.IsTrue((string)document.Fields["nick"] == "ThePresident");

            Assert.IsTrue(document.Fields["follows"].GetType() == typeof(List<object>));

            Assert.IsTrue(document.Fields["followers"].GetType() == typeof(List<object>));
            List<object> followers = (List<object>)document.Fields["followers"];

            Assert.IsTrue(followers[0].GetType() == typeof(string));
            Assert.IsTrue((string)followers[0] == "#10:5");

            Assert.IsTrue(followers[1].GetType() == typeof(string));
            Assert.IsTrue((string)followers[1] == "#10:6");

            Assert.IsTrue(document.Fields["name"].GetType() == typeof(string));
            Assert.IsTrue((string)document.Fields["name"] == "Barack");

            Assert.IsTrue(document.Fields["surname"].GetType() == typeof(string));
            Assert.IsTrue((string)document.Fields["surname"] == "Obama");

            Assert.IsTrue(document.Fields["location"].GetType() == typeof(string));
            Assert.IsTrue((string)document.Fields["location"] == "#3:2");

            Assert.IsTrue(document.Fields["invitedBy"] == null);

            Assert.IsTrue(document.Fields["salary_cloned"] == null);

            Assert.IsTrue(document.Fields["salary"].GetType() == typeof(float));
            Assert.IsTrue((float)document.Fields["salary"] == 120.3f);
        }

        [TestMethod]
        public void TestExample2()
        {
            string raw = "name:\"ORole\",id:0,defaultClusterId:3,clusterIds:[3],properties:[(name:\"mode\",type:17,offset:0,mandatory:false,notNull:false,min:,max:,linkedClass:,linkedType:,index:),(name:\"rules\",type:12,offset:1,mandatory:false,notNull:false,min:,max:,linkedClass:,linkedType:17,index:)]";

            ORecord record = new ORecord(ORecordType.Document, 0, UTF8Encoding.UTF8.GetBytes(raw));
            ODocument document = record.ToDocument();

            Assert.IsTrue(document.Fields["name"].GetType() == typeof(string));
            Assert.IsTrue((string)document.Fields["name"] == "ORole");

            Assert.IsTrue(document.Fields["id"].GetType() == typeof(int));
            Assert.IsTrue((int)document.Fields["id"] == 0);

            Assert.IsTrue(document.Fields["defaultClusterId"].GetType() == typeof(int));
            Assert.IsTrue((int)document.Fields["defaultClusterId"] == 3);

            Assert.IsTrue(document.Fields["properties"].GetType() == typeof(List<object>));
            List<object> properties = (List<object>)document.Fields["properties"];

            Dictionary<string, object> embedded = (Dictionary<string, object>)properties[0];

            Assert.IsTrue(embedded["name"].GetType() == typeof(string));
            Assert.IsTrue((string)embedded["name"] == "mode");

            Assert.IsTrue(embedded["type"].GetType() == typeof(int));
            Assert.IsTrue((int)embedded["type"] == 17);

            Assert.IsTrue(embedded["offset"].GetType() == typeof(int));
            Assert.IsTrue((int)embedded["offset"] == 0);

            Assert.IsTrue(embedded["mandatory"].GetType() == typeof(bool));
            Assert.IsTrue((bool)embedded["mandatory"] == false);

            Assert.IsTrue(embedded["notNull"].GetType() == typeof(bool));
            Assert.IsTrue((bool)embedded["notNull"] == false);

            Assert.IsTrue(embedded["min"] == null);

            Assert.IsTrue(embedded["max"] == null);

            Assert.IsTrue(embedded["linkedClass"] == null);

            Assert.IsTrue(embedded["linkedType"] == null);

            Assert.IsTrue(embedded["index"] == null);

            embedded = (Dictionary<string, object>)properties[1];

            Assert.IsTrue(embedded["name"].GetType() == typeof(string));
            Assert.IsTrue((string)embedded["name"] == "rules");

            Assert.IsTrue(embedded["type"].GetType() == typeof(int));
            Assert.IsTrue((int)embedded["type"] == 12);

            Assert.IsTrue(embedded["offset"].GetType() == typeof(int));
            Assert.IsTrue((int)embedded["offset"] == 1);

            Assert.IsTrue(embedded["mandatory"].GetType() == typeof(bool));
            Assert.IsTrue((bool)embedded["mandatory"] == false);

            Assert.IsTrue(embedded["notNull"].GetType() == typeof(bool));
            Assert.IsTrue((bool)embedded["notNull"] == false);

            Assert.IsTrue(embedded["min"] == null);

            Assert.IsTrue(embedded["max"] == null);

            Assert.IsTrue(embedded["linkedClass"] == null);

            Assert.IsTrue(embedded["linkedType"].GetType() == typeof(int));
            Assert.IsTrue((int)embedded["linkedType"] == 17);

            Assert.IsTrue(embedded["index"] == null);
        }
    }
}
