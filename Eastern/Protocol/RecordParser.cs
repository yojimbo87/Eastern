using System.Linq;
using System.Collections.Generic;

namespace Eastern.Protocol
{
    internal static class RecordParser
    {
        internal static ODocument ToDocument(string rawDocument)
        {
            ODocument document = new ODocument();

            int atIndex = rawDocument.IndexOf('@');
            int colonIndex = rawDocument.IndexOf(':');

            string parsedType = "fieldName";
            string lastParsedField = "";
            int itemStartIndex = 0;
            RecordColletionType collectionType = RecordColletionType.None;

            // parse class name
            if ((atIndex != -1) && (atIndex < colonIndex))
            {
                document.Class = rawDocument.Substring(0, atIndex);
                itemStartIndex = atIndex + 1;
            }
            else
            {
                document.Class = "";
            }

            int i = 0;

            while (i < rawDocument.Length)
            {
                switch (rawDocument[i])
                {

                    case ':':
                        if (parsedType == "fieldName")
                        {
                            lastParsedField = rawDocument.Substring(itemStartIndex, i - itemStartIndex);
                            document.Fields.Add(lastParsedField, null);

                            parsedType = "value";
                            itemStartIndex = i + 1;
                        }
                        break;
                    case ',':
                        // first parse value
                        switch (parsedType)
                        {
                            case "recordID":
                            case "value":
                                string value = rawDocument.Substring(itemStartIndex, i - itemStartIndex);

                                if (collectionType == RecordColletionType.Flat)
                                {
                                    ((List<string>)document.Fields[lastParsedField]).Add(rawDocument.Substring(itemStartIndex, i - itemStartIndex));
                                }
                                else 
                                {
                                    if (value != "")
                                    {
                                        document.Fields[lastParsedField] = rawDocument.Substring(itemStartIndex, i - itemStartIndex);
                                    }
                                }

                                itemStartIndex = i + 1;

                                if (parsedType == "recordID")
                                {
                                    parsedType = "";
                                }
                                break;
                            default:
                                break;
                        }

                        // then examine if the fieldName parsing follows
                        if ((parsedType != "string") && (parsedType != "recordID") && (collectionType == RecordColletionType.None))
                        {
                            parsedType = "fieldName";
                            itemStartIndex = i + 1;
                        }
                        break;
                    case '"':
                        if (parsedType != "string")
                        {
                            parsedType = "string";
                            itemStartIndex = i + 1;
                        }
                        else
                        {
                            string value = rawDocument.Substring(itemStartIndex, i - itemStartIndex);

                            if (collectionType == RecordColletionType.Flat)
                            {
                                ((List<string>)document.Fields[lastParsedField]).Add(value);
                            }
                            else 
                            {
                                document.Fields[lastParsedField] = value;
                            }

                            parsedType = "";
                        }
                        break;
                    case '#':
                        if (parsedType != "string")
                        {
                            if (parsedType != "recordID")
                            {
                                parsedType = "recordID";
                                itemStartIndex = i;
                            }
                        }
                        break;
                    case '(':
                        if (parsedType != "string")
                        {
                            //collectionType = RecordColletionType.NestedDocuments;
                            parsedType = "fieldName";
                            itemStartIndex = i + 1;

                            /*if (collectionType == RecordColletionType.NestedDocuments)
                            {
                                document.Fields[lastParsedField] = new Dictionary<string, object>();
                            }*/
                        }
                        break;
                    case ')':
                        if (parsedType != "string")
                        {
                            switch (parsedType)
                            {
                                case "recordID":
                                case "value":
                                    string value = rawDocument.Substring(itemStartIndex, i - itemStartIndex);

                                    if (collectionType == RecordColletionType.Flat)
                                    {
                                        ((List<string>)document.Fields[lastParsedField]).Add(value);
                                    }
                                    else
                                    {
                                        if (value != "")
                                        {
                                            document.Fields[lastParsedField] = value;
                                        }
                                    }

                                    parsedType = "";
                                    break;
                                default:
                                    break;
                            }

                            
                            parsedType = "";
                        }
                        break;
                    case '[':
                        if (parsedType != "string")
                        {
                            // check if the collection is flat or consists of nested docuemnts
                            if (rawDocument[i + 1] == '(')
                            {
                                collectionType = RecordColletionType.NestedDocuments;
                            }
                            else
                            {
                                document.Fields[lastParsedField] = new List<string>();
                                collectionType = RecordColletionType.Flat;
                            }

                            itemStartIndex = i + 1;
                        }
                        break;
                    case ']':
                        if ((parsedType != "string") && (collectionType != RecordColletionType.None))
                        {
                            switch (parsedType)
                            {
                                case "recordID":
                                case "value":
                                    string value = rawDocument.Substring(itemStartIndex, i - itemStartIndex);
                                    ((List<string>)document.Fields[lastParsedField]).Add(value);
                                    parsedType = "";
                                    break;
                                default:
                                    break;
                            }

                            parsedType = "";
                            collectionType = RecordColletionType.None;
                        }
                        break;
                    default:
                        break;
                }

                i++;

                // parsing of last value at the end of raw document string
                if (i == rawDocument.Length)
                {
                    switch (parsedType)
                    {
                        case "recordID":
                        case "value":
                            string value = rawDocument.Substring(itemStartIndex, i - itemStartIndex);

                            if (collectionType == RecordColletionType.Flat)
                            {
                                ((List<string>)document.Fields[lastParsedField]).Add(value);
                            }
                            else
                            {
                                if (value != "")
                                {
                                    document.Fields[lastParsedField] = value;
                                }
                            }
                            break;
                        default:
                            break;
                    }
                }
            }

            return document;
        }
    }

    internal enum RecordColletionType
    {
        None = 0,
        Flat = 1,
        NestedDocuments = 2
    }
}
