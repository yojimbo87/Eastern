
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
                return "#" + ClusterID + ":" + ClusterPosition;
            }

            set
            {
                string[] split = value.Split(':');

                ClusterID = short.Parse(split[0].Substring(1));
                ClusterPosition = long.Parse(split[1]);
            } 
        }

        public ORID()
        {

        }

        /// <summary>
        /// Creates ORID reference with given cluster ID and position.
        /// </summary>
        public ORID(short clusterID, long clusterPosition)
        {
            ClusterID = clusterID;
            ClusterPosition = clusterPosition;
        }

        /// <summary>
        /// Creates ORID reference from given string (in the form of "#clusterID:clusterPosition").
        /// </summary>
        public ORID(string oridString)
        {
            string[] split = oridString.Split(':');

            ClusterID = short.Parse(split[0].Substring(1));
            ClusterPosition = long.Parse(split[1]);
        }
    }
}
