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

                            parsedType = "";
                        }
                        break;
                    case ',':
                        // first parse value
                        switch (parsedType)
                        {
                            case "recordID":
                                document.Fields[lastParsedField] = rawDocument.Substring(itemStartIndex, i - itemStartIndex);
                                parsedType = "";
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
                            document.Fields[lastParsedField] = rawDocument.Substring(itemStartIndex, i - itemStartIndex);
                            parsedType = "";
                        }
                        break;
                    case '#':
                        if (parsedType != "string")
                        {
                            if (parsedType != "recordID")
                            {
                                parsedType = "recordID";
                                itemStartIndex = i + 1;
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
                            parsedType = "";
                        }
                        break;
                    case '[':
                        if (parsedType != "string")
                        {
                            if (rawDocument[i + 1] != '(')
                            {
                                isParsingFlatCollection = true;
                            }
                            else
                            {

                            }
                        }
                        break;
                    case ']':
                        if ((parsedType != "string") && isParsingFlatCollection)
                        {
                            parsedType = "";
                            isParsingFlatCollection = false;
                        }
                        break;
                    default:
                        break;
                }

                i++;

                if (i == rawDocument.Length)
                {
                    switch (parsedType)
                    {
                        case "recordID":
                            document.Fields[lastParsedField] = rawDocument.Substring(itemStartIndex, i - itemStartIndex);
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
