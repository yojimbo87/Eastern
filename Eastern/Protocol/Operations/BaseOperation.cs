using Eastern.Protocol;

namespace Eastern.Protocol.Operations
{
    internal abstract class BaseOperation
    {
        internal OperationType OperationType { get; set; }
        internal int SessionID { get; set; }
    }
}
