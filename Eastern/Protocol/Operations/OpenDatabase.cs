using System.Collections.Generic;
using System.Linq;
using Eastern.Connection;

namespace Eastern.Protocol.Operations
{
    internal class OpenDatabase : BaseOperation, IOperation
    {
        internal string DriverName { get { return "Eastern"; } }
        internal string DriverVersion { get { return "0.0.1 pre-alpha"; } }
        internal short ProtocolVersion { get; set; }
        internal string ClientID { get { return "null"; } }

        internal string DatabaseName { get; set; }
        internal DatabaseType DatabaseType { get; set; }
        internal string UserName { get; set; }
        internal string UserPassword { get; set; }
        internal string ClusterConfig { get { return "null"; } }

        internal OpenDatabase()
        {
            OperationType = OperationType.DB_OPEN;
        }

        public Request Request()
        {
            Request request = new Connection.Request();
            // standard request fields
            request.DataItems.Add(new DataItem() { Type = "byte", Data = Parser.ToArray((byte)OperationType) });
            request.DataItems.Add(new DataItem() { Type = "int", Data = Parser.ToArray(SessionID) });
            // operation specific fields
            request.DataItems.Add(new DataItem() { Type = "string", Data = Parser.ToArray(DriverName) });
            request.DataItems.Add(new DataItem() { Type = "string", Data = Parser.ToArray(DriverVersion) });
            request.DataItems.Add(new DataItem() { Type = "short", Data = Parser.ToArray(ProtocolVersion) });
            request.DataItems.Add(new DataItem() { Type = "string", Data = Parser.ToArray(ClientID) });
            request.DataItems.Add(new DataItem() { Type = "string", Data = Parser.ToArray(DatabaseName) });
            request.DataItems.Add(new DataItem() { Type = "string", Data = Parser.ToArray((DatabaseType == DatabaseType.Document) ? "document" : "graph") });
            request.DataItems.Add(new DataItem() { Type = "string", Data = Parser.ToArray(UserName) });
            request.DataItems.Add(new DataItem() { Type = "string", Data = Parser.ToArray(UserPassword) });

            return request;
        }

        public object Response(Response response)
        {
            int offset = 1;
            ODatabase database = new ODatabase();

            // standard response fields
            response.Status = (ResponseStatus)Parser.ToByte(response.Data.Take(1).ToArray());
            response.SessionID = Parser.ToInt(response.Data.Skip(offset).Take(4).ToArray());
            offset += 4;
            // operation specific fields
            database.SessionID = Parser.ToInt(response.Data.Skip(offset).Take(4).ToArray());
            offset += 4;
            database.ClustersCount = Parser.ToShort(response.Data.Skip(offset).Take(2).ToArray());
            offset += 2;

            if (database.ClustersCount > 0)
            {
                for (int i = 1; i <= database.ClustersCount; i++)
                {
                    Cluster cluster = new Cluster();
                    
                    int clusterNameLength = Parser.ToInt(response.Data.Skip(offset).Take(4).ToArray());
                    offset += 4;
                    
                    cluster.Name = Parser.ToString(response.Data.Skip(offset).Take(clusterNameLength).ToArray());
                    offset += clusterNameLength;
                    
                    cluster.ID = Parser.ToShort(response.Data.Skip(offset).Take(2).ToArray());
                    offset += 2;

                    int clusterTypeLength = Parser.ToInt(response.Data.Skip(offset).Take(4).ToArray());
                    offset += 4;

                    cluster.Type = Parser.ToString(response.Data.Skip(offset).Take(clusterTypeLength).ToArray());
                    offset += clusterTypeLength;

                    cluster.DataSegmentID = Parser.ToShort(response.Data.Skip(offset).Take(2).ToArray());
                    offset += 2;

                    database.Clusters.Add(cluster);
                }
            }

            int clusterConfigLength = Parser.ToInt(response.Data.Skip(offset).Take(4).ToArray());
            offset += 4;

            database.ClusterConfig = response.Data.Skip(offset).Take(clusterConfigLength).ToArray();
            offset += clusterConfigLength;

            return database;
        }
    }
}
