﻿using System.Collections.Generic;
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
            request.DataItems.Add(new DataItem() { Type = "byte", Data = BinaryParser.ToArray((byte)OperationType) });
            request.DataItems.Add(new DataItem() { Type = "int", Data = BinaryParser.ToArray(SessionID) });
            // operation specific fields
            request.DataItems.Add(new DataItem() { Type = "string", Data = BinaryParser.ToArray(DriverName) });
            request.DataItems.Add(new DataItem() { Type = "string", Data = BinaryParser.ToArray(DriverVersion) });
            request.DataItems.Add(new DataItem() { Type = "short", Data = BinaryParser.ToArray(ProtocolVersion) });
            request.DataItems.Add(new DataItem() { Type = "string", Data = BinaryParser.ToArray(ClientID) });
            request.DataItems.Add(new DataItem() { Type = "string", Data = BinaryParser.ToArray(DatabaseName) });
            request.DataItems.Add(new DataItem() { Type = "string", Data = BinaryParser.ToArray((DatabaseType == DatabaseType.Document) ? "document" : "graph") });
            request.DataItems.Add(new DataItem() { Type = "string", Data = BinaryParser.ToArray(UserName) });
            request.DataItems.Add(new DataItem() { Type = "string", Data = BinaryParser.ToArray(UserPassword) });

            return request;
        }

        public object Response(Response response)
        {
            int offset = 1;
            ODatabase database = new ODatabase();

            // standard response fields
            response.Status = (ResponseStatus)BinaryParser.ToByte(response.Data.Take(1).ToArray());
            response.SessionID = BinaryParser.ToInt(response.Data.Skip(offset).Take(4).ToArray());
            offset += 4;
            // operation specific fields
            database.SessionID = BinaryParser.ToInt(response.Data.Skip(offset).Take(4).ToArray());
            offset += 4;
            database.ClustersCount = BinaryParser.ToShort(response.Data.Skip(offset).Take(2).ToArray());
            offset += 2;

            if (database.ClustersCount > 0)
            {
                for (int i = 1; i <= database.ClustersCount; i++)
                {
                    Cluster cluster = new Cluster();

                    int clusterNameLength = BinaryParser.ToInt(response.Data.Skip(offset).Take(4).ToArray());
                    offset += 4;

                    cluster.Name = BinaryParser.ToString(response.Data.Skip(offset).Take(clusterNameLength).ToArray());
                    offset += clusterNameLength;

                    cluster.ID = BinaryParser.ToShort(response.Data.Skip(offset).Take(2).ToArray());
                    offset += 2;

                    int clusterTypeLength = BinaryParser.ToInt(response.Data.Skip(offset).Take(4).ToArray());
                    offset += 4;

                    cluster.Type = BinaryParser.ToString(response.Data.Skip(offset).Take(clusterTypeLength).ToArray());
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
