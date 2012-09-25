
namespace Eastern
{
    public class ORID
    {
        public short ClusterID { get; set; }
        public long ClusterPosition { get; set; }
        public string RID 
        {
            get
            {
                return ClusterID + ":" + ClusterPosition;
            }

            set
            {
                string[] split = value.Split(':');

                ClusterID = short.Parse(split[0]);
                ClusterPosition = long.Parse(split[1]);
            } 
        }
    }
}
