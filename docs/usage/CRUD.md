CRUD record operations
---

Eastern supports raw and strongly typed (POCO) create, read (or load in OrientDB context), update and delete operations for single records. List of supported CRUD operations can be found in [ODatabase API](https://github.com/yojimbo87/Eastern/blob/master/docs/api/ODatabase.md).

    // example of POCO object to be serialized
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

    ...

    // create instance of previously defined POCO with data
    TestClass foo = new TestClass();
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
    
    // connect to database (with the use of connection pool)
    using (ODatabase database = new ODatabase("yourDatabaseAlias"))
    {
        // create record within TestClass cluster
        ORecord recordCreated = database.CreateRecord("TestClass", foo);
        
        // load previously created POCO which should contain identical data with foo
        TestClass fooRetrieved = database.LoadRecord<TestClass>(recordCreated.ORID);
        
        // change some data
        fooRetrieved.String = "new string value";
        
        // update existing record
        int newVersion = database.UpdateRecord(recordCreated.ORID, fooRetrieved);
        
        // delete existing record
        bool isRecordDeleted = database.DeleteRecord(recordCreated.ORID);
    }