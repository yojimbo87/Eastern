using Eastern.Connection;
using Eastern.Protocol.Operations;

namespace Eastern
{
    public class EasternClient
    {
        internal static string DriverName { get { return "Eastern"; } }
        internal static string DriverVersion { get { return "0.0.1 pre-alpha"; } }
        internal static short ProtocolVersion { get { return 12; } }
        internal static string ClientID { get { return "null"; } }

        private Worker WorkerConnection { get; set; }

        public EasternClient()
        {
            WorkerConnection = new Worker();
        }

        public EasternClient(string hostname, int port)
        {
            WorkerConnection = new Worker();
            WorkerConnection.Initialize(hostname, port);
        }

        /*internal static object QueueOperation<T>(T operation)
        {
            return WorkerConnection.ExecuteOperation<T>(operation);
        }*/
    }
}
