﻿using System.Linq;
using Eastern.Connection;

namespace Eastern.Protocol.Operations
{
    internal class Command : IOperation
    {
        internal OperationMode OperationMode { get; set; }
        internal CommandClassType ClassType { get; set; }
        internal CommandPayload CommandPayload { get; set; }

        public Request Request(int sessionID)
        {
            Request request = new Connection.Request();

            // standard request fields
            request.DataItems.Add(new DataItem() { Type = "byte", Data = BinaryParser.ToArray((byte)OperationType.COMMAND) });
            request.DataItems.Add(new DataItem() { Type = "int", Data = BinaryParser.ToArray(sessionID) });
            // operation specific fields
            request.DataItems.Add(new DataItem() { Type = "byte", Data = BinaryParser.ToArray((byte)OperationMode) });

            // class name field
            string className = "default";
            switch (ClassType)
            {
                // idempotent command (e.g. select)
                case CommandClassType.Idempotent:
                    className = "q";
                    break;
                // non-idempotent command (e.g. insert)
                case CommandClassType.NonIdempotent:
                    className = "c";
                    break;
                // script command
                case CommandClassType.Script:
                    className = "s";
                    break;
                default:
                    className = "default";
                    break;
            }
            request.DataItems.Add(new DataItem() { Type = "string", Data = BinaryParser.ToArray(className) });

            if (CommandPayload.Type == CommandPayloadType.SqlScript)
            {
                request.DataItems.Add(new DataItem() { Type = "string", Data = BinaryParser.ToArray(CommandPayload.Language) });
            }

            request.DataItems.Add(new DataItem() { Type = "string", Data = BinaryParser.ToArray(CommandPayload.Text) });
            request.DataItems.Add(new DataItem() { Type = "int", Data = BinaryParser.ToArray(CommandPayload.NonTextLimit) });
            request.DataItems.Add(new DataItem() { Type = "string", Data = BinaryParser.ToArray(CommandPayload.FetchPlan) });
            request.DataItems.Add(new DataItem() { Type = "bytes", Data = CommandPayload.SerializedParams });

            return request;
        }

        public object Response(Response response)
        {
            // start from this position since standard fields (status, session ID) has been already parsed
            int offset = 5;
            DtoCommand command = new DtoCommand();

            if (response == null)
            {
                return command;
            }

            // operation specific fields
            command.PayloadStatus = (PayloadStatus)BinaryParser.ToByte(response.Data.Skip(offset).Take(1).ToArray());
            offset += 1;

            if (OperationMode == OperationMode.Asynchronous)
            {
                while (command.PayloadStatus != PayloadStatus.NoRemainingRecords)
                {
                    switch (command.PayloadStatus)
                    {
                        case PayloadStatus.ResultSet:

                            break;
                        case PayloadStatus.PreFetched:
                            // TODO:
                            break;
                        default:
                            break;
                    }

                    command.PayloadStatus = (PayloadStatus)BinaryParser.ToByte(response.Data.Skip(offset).Take(1).ToArray());
                    offset += 1;
                }
            }
            else
            {
                switch (command.PayloadStatus)
                {
                    case PayloadStatus.NullResult:
                        command.Content = null;
                        break;
                    case PayloadStatus.SingleRecord:

                        break;
                    case PayloadStatus.SerializedResult:

                        break;
                    case PayloadStatus.RecordCollection:

                        break;
                    default:
                        break;
                }
            }

            return command;
        }
    }
}
