using System.Collections.Generic;

using Eastern.Connection;

namespace Eastern.Protocol.Operations
{
    internal interface IOperation
    {
        Request Request(int sessionID);
        object Response(Response response);
    }
}
