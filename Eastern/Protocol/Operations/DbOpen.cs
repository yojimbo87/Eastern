using System;
using System.Collections.Generic;
using System.Linq;
using Eastern.Connection;

namespace Eastern.Protocol.Operations
{
    internal class DbOpen : IOperation
    {
        internal string DatabaseName { get; set; }
        internal ODatabaseType DatabaseType { get; set; }
        internal string UserName { get; set; }
        internal string UserPassword { get; set; }
        internal string ClusterConfig { get { return "null"; } }

        public Request Request(int sessionID)
        {
            Request request = new Connection.Request();
            // standard request fields
            request.DataItems.Add(new DataItem() { Type = "byte", Data = BinaryParser.ToArray((byte)OperationType.DB_OPEN) });
            request.DataItems.Add(new DataItem() { Type = "int", Data = BinaryParser.ToArray(sessionID) });
            // operation specific fields
            request.DataItems.Add(new DataItem() { Type = "string", Data = BinaryParser.ToArray(EasternClient.DriverName) });
            request.DataItems.Add(new DataItem() { Type = "string", Data = BinaryParser.ToArray(EasternClient.DriverVersion) });
            request.DataItems.Add(new DataItem() { Type = "short", Data = BinaryParser.ToArray(EasternClient.ProtocolVersion) });
            request.DataItems.Add(new DataItem() { Type = "string", Data = BinaryParser.ToArray(EasternClient.ClientID) });
            request.DataItems.Add(new DataItem() { Type = "string", Data = BinaryParser.ToArray(DatabaseName) });
            request.DataItems.Add(new DataItem() { Type = "string", Data = BinaryParser.ToArray(DatabaseType.ToString().ToLower()) });
            request.DataItems.Add(new DataItem() { Type = "string", Data = BinaryParser.ToArray(UserName) });
            request.DataItems.Add(new DataItem() { Type = "string", Data = BinaryParser.ToArray(UserPassword) });

            return request;
        }

        public object Response(Response response)
        {
            // start from this position since standard fields (status, session ID) has been already parsed
            int offset = 5;
            ODatabase database = new ODatabase();

            if (response == null)
            {
                return database;
            }

            // operation specific fields
            database.SessionID = BinaryParser.ToInt(response.Data.Skip(offset).Take(4).ToArray());
            offset += 4;
            database.ClustersCount = BinaryParser.ToShort(response.Data.Skip(offset).Take(2).ToArray());
            offset += 2;

            if (database.ClustersCount > 0)
            {
                for (int i = 1; i <= database.ClustersCount; i++)
                {
                    OCluster cluster = new OCluster();

                    int clusterNameLength = BinaryParser.ToInt(response.Data.Skip(offset).Take(4).ToArray());
                    offset += 4;

                    cluster.Name = BinaryParser.ToString(response.Data.Skip(offset).Take(clusterNameLength).ToArray());
                    offset += clusterNameLength;

                    cluster.ID = BinaryParser.ToShort(response.Data.Skip(offset).Take(2).ToArray());
                    offset += 2;

                    int clusterTypeLength = BinaryParser.ToInt(response.Data.Skip(offset).Take(4).ToArray());
                    offset += 4;

                    string clusterName = BinaryParser.ToString(response.Data.Skip(offset).Take(clusterTypeLength).ToArray());
                    cluster.Type = (OClusterType)Enum.Parse(typeof(OClusterType), clusterName, true);
                    offset += clusterTypeLength;

                    cluster.DataSegmentID = BinaryParser.ToShort(response.Data.Skip(offset).Take(2).ToArray());
                    offset += 2;

                    database.Clusters.Add(cluster);
                }
            }

            int clusterConfigLength = BinaryParser.ToInt(response.Data.Skip(offset).Take(4).ToArray());
            offset += 4;

            database.ClusterConfig = response.Data.Skip(offset).Take(clusterConfigLength).ToArray();
            offset += clusterConfigLength;

            return database;
        }
    }
}
