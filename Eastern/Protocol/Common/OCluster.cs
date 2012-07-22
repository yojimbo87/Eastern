
namespace Eastern
{
    public class OCluster
    {
        public short ID { get; set; }
        public string Name { get; set; }
        public OClusterType Type { get; set; }
        public string Location { get; set; }
        public short DataSegmentID { get; set; }
        public string DataSegmentName { get; set; }
    }
}
