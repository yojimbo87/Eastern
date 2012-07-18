using Eastern.Protocol;

namespace Eastern.Protocol.Operations
{
    internal abstract class BaseOperation
    {
        internal OperationType OperationType { get; set; }
        internal int SessionID { get; set; }

        internal string DriverName { get { return "Eastern"; } }
        internal string DriverVersion { get { return "0.0.1 pre-alpha"; } }
        internal string ClientID { get { return "null"; } }
        internal short ProtocolVersion { get; set; }
        internal BinaryParser Parser { get; set; }

        internal BaseOperation()
        {
            Parser = new BinaryParser();
        }
    }
}
