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
        public void TestDocumentWithClass()
        {
            string rawDocument = "Profile@nick:\"ThePresident\",follows:[],followers:[#10:5,#10:6],name:\"Barack\",surname:\"Obama\",location:#3:2,invitedBy:,salary_cloned:,salary:120.3f";

            ORecord record = new ORecord(ORecordType.Document, 0, UTF8Encoding.UTF8.GetBytes(rawDocument));
            ODocument document = record.ToDocument();

            Assert.IsTrue(document.Class.Equals("Profile"));
        }

        [TestMethod]
        public void TestDocumentWithoutClass()
        {
            string rawDocument = "name:\"ORole\",id:0,defaultClusterId:3,clusterIds:[3],properties:[(name:\"mode\",type:17,offset:0,mandatory:false,notNull:false,min:,max:,linkedClass:,linkedType:,index:#),name:\"rules\",type:12,offset:1,mandatory:false,notNull:false,min:,max:,linkedClass:,linkedType:17,index:#)]";

            ORecord record = new ORecord(ORecordType.Document, 0, UTF8Encoding.UTF8.GetBytes(rawDocument));
            ODocument document = record.ToDocument();

            Assert.IsTrue(document.Class.Equals(""));
        }
    }
}
