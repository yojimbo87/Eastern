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
            bool isParsingFlatCollection = false;

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

                                if (isParsingFlatCollection)
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
                        if ((parsedType != "string") && (parsedType != "recordID") && !isParsingFlatCollection)
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

                            if (isParsingFlatCollection)
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
                            parsedType = "fieldName";
                            itemStartIndex = i + 1;
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

                                    if (isParsingFlatCollection)
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
                            if (rawDocument[i + 1] != '(')
                            {
                                document.Fields[lastParsedField] = new List<string>();
                                isParsingFlatCollection = true;
                                itemStartIndex = i + 1;
                            }
                            else
                            {

                            }
                        }
                        break;
                    case ']':
                        if ((parsedType != "string") && isParsingFlatCollection)
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
                            isParsingFlatCollection = false;
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

                            if (isParsingFlatCollection)
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
}
