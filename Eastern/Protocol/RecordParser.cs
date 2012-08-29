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
                            document.Fields.Add(rawDocument.Substring(itemStartIndex, i - itemStartIndex), new object ());

                            parsedType = "";
                        }
                        break;
                    case ',':
                        if (parsedType != "string" && !isParsingFlatCollection)
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
                            else
                            {
                                parsedType = "";
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
            }

            return document;
        }
    }
}
