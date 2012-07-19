using Eastern.Protocol;

namespace Eastern.Protocol.Operations
{
    internal abstract class BaseOperation
    {
        internal OperationType OperationType { get; set; }
        internal int SessionID { get; set; }

        internal BinaryParser Parser { get; set; }

        internal BaseOperation()
        {
            Parser = new BinaryParser();
        }
    }
}
