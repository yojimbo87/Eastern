using System;
using System.Collections.Generic;
using System.Linq;
using Eastern.Connection;

namespace Eastern.Protocol.Operations
{
    internal class DbReload : IOperation
    {
        public Request Request(int sessionID)
        {
            Request request = new Connection.Request();
            // standard request fields
            request.DataItems.Add(new DataItem() { Type = "byte", Data = BinaryParser.ToArray((byte)OperationType.DB_RELOAD) });
            request.DataItems.Add(new DataItem() { Type = "int", Data = BinaryParser.ToArray(sessionID) });

            return request;
        }

        public object Response(Response response)
        {
            // start from this position since standard fields (status, session ID) has been already parsed
            int offset = 5;
            Database database = new Database();

            if (response == null)
            {
                return database;
            }

            // operation specific fields
            database.ClustersCount = BinaryParser.ToShort(response.Data.Skip(offset).Take(2).ToArray());
            offset += 2;

            //System.Console.WriteLine(BinaryParser.ToString(response.Data));

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

            return database;
        }
    }
}
