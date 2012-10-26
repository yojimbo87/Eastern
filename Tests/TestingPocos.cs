using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Tests
{
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
