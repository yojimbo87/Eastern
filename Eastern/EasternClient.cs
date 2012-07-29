using Eastern.Connection;
using Eastern.Protocol.Operations;

namespace Eastern
{
    public static class EasternClient
    {
        internal static string DriverName { get { return "Eastern"; } }
        internal static string DriverVersion { get { return "0.0.1 pre-alpha"; } }
        internal static short ProtocolVersion { get { return 12; } }
        internal static string ClientID { get { return "null"; } }

        static EasternClient()
        {
            
        }
    }
}
