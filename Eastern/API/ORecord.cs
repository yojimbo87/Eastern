using System;
using System.Globalization;
using System.Collections.Generic;
using Eastern.Protocol;

namespace Eastern
{
    public class ORecord
    {
        private ODocument Document { get; set; }
        private string RawDocument { get; set; }

        public ORecordType Type { get; set; }
        public int Version { get; set; }
        public byte[] Content { get; set; }

        public ORecord(ORecordType type, int version, byte[] content)
        {
            Type = type;
            Version = version;
            Content = content;
        }

        internal ORecord(Record record)
        {
            Type = record.Type;
            Version = record.Version;
            Content = record.Content;
        }

        public ODocument ToDocument()
        {
            if (Type != ORecordType.Document)
            {
                return null;
            }

            Document = new ODocument();
            Document.Version = Version;
            RawDocument = BinaryParser.ToString(Content);

            int atIndex = RawDocument.IndexOf('@');
            int colonIndex = RawDocument.IndexOf(':');
            int index = 0;

            // parse class name
            if ((atIndex != -1) && (atIndex < colonIndex))
            {
                Document.Class = RawDocument.Substring(0, atIndex);
                index = atIndex + 1;
            }
            else
            {
                Document.Class = "";
            }

            // start document parsing with first field name
            do
            {
                index = ParseFieldName(index, null);
            }
            while (index < RawDocument.Length);

            return Document;
        }

        private int ParseFieldName(int i, Dictionary<string, object> fields)
        {
            int startIndex = i;

            // iterate until colon is found since it's the character which ends the field name
            while (RawDocument[i] != ':')
            {
                i++;
            }

            // parse field name string from raw document
            string fieldName = RawDocument.Substring(startIndex, i - startIndex);
            Dictionary<string, object> currentDocument;

            // if fields were not passed - current document is the root one
            // otherwise - it's embedded document passed as fields parameter
            if (fields == null)
            {
                currentDocument = Document.Fields;
            }
            else
            {
                currentDocument = fields;
            }

            // add parsed field name to current document
            currentDocument.Add(fieldName, null);

            // move to position after colon (:)
            i++;

            // check if it's not the end of document which means that current field has null value
            if (i == RawDocument.Length)
            {
                return i;
            }

            // check what follows after parsed field name and start parsing underlying type
            switch (RawDocument[i])
            {
                case '"':
                    i = ParseString(i, currentDocument, fieldName);
                    break;
                case '#':
                    i = ParseRecordID(i, currentDocument, fieldName);
                    break;
                case '(':
                    i = ParseEmbeddedDocument(i, currentDocument, fieldName);
                    break;
                case '[':
                    if (RawDocument[i + 1] == '(')
                    {
                        i = ParseCollection(i, currentDocument, fieldName);
                    }
                    else
                    {
                        i = ParseCollection(i, currentDocument, fieldName);
                    }
                    break;
                case '{':
                    i = ParseMap(i, currentDocument, fieldName);
                    break;
                default:
                    i = ParseValue(i, currentDocument, fieldName);
                    break;
            }

            // check if it's not the end of document which means that current field has null value
            if (i == RawDocument.Length)
            {
                return i;
            }

            // single string value was parsed and we need to push the index if next character is comma
            if (RawDocument[i] == ',')
            {
                i++;
            }

            return i;
        }

        private int ParseString(int i, Dictionary<string, object> document, string fieldName)
        {
            // move to the inside of string
            i++;

            int startIndex = i;

            // search for end of the parsed string value
            while (RawDocument[i] != '"')
            {
                // strings must escape these characters:
                // " -> \"
                // \ -> \\
                // therefore there needs to be a check for valid end of the string which
                // is quote character that is not preceeded by backslash character \
                if ((RawDocument[i] == '\\') && (RawDocument[i + 1] == '"'))
                {
                    i = i + 2;
                }
                else 
                {
                    i++;
                }
            }

            string value = RawDocument.Substring(startIndex, i - startIndex);
            // escape quotes
            value = value.Replace("\\" + "\"", "\"");
            // escape backslashes
            value = value.Replace("\\\\", "\\");

            // assign field value
            if (document[fieldName] == null)
            {
                document[fieldName] = value;
            }
            else
            {
                ((List<object>)document[fieldName]).Add(value);
            }

            // move past the closing quote character
            i++;

            return i;
        }

        private int ParseRecordID(int i, Dictionary<string, object> document, string fieldName)
        {
            int startIndex = i;

            // search for end of parsed record ID value
            while ((i < RawDocument.Length) && (RawDocument[i] != ',') && (RawDocument[i] != ')') && (RawDocument[i] != ']'))
            {
                i++;
            }

            //assign field value
            if (document[fieldName] == null)
            {
                document[fieldName] = RawDocument.Substring(startIndex, i - startIndex);
            }
            else
            {
                ((List<object>)document[fieldName]).Add(RawDocument.Substring(startIndex, i - startIndex));
            }

            return i;
        }

        private int ParseMap(int i, Dictionary<string, object> document, string fieldName)
        {
            int startIndex = i;
            int nestingLevel = 1;

            // search for end of parsed map
            while ((i < RawDocument.Length) && (nestingLevel != 0))
            {
                // check for beginning of the string to prevent finding an end of map within string value
                if (RawDocument[i + 1] == '"')
                {
                    // move to the beginning of the string
                    i++;

                    // go to the end of string
                    while ((i < RawDocument.Length) && (RawDocument[i] != '"'))
                    {
                        i++;
                    }

                    // move to the end of string
                    i++;
                }
                else if (RawDocument[i + 1] == '{')
                {
                    // move to the beginning of the string
                    i++;

                    nestingLevel++;
                }
                else if (RawDocument[i + 1] == '}')
                {
                    // move to the beginning of the string
                    i++;

                    nestingLevel--;
                }
                else
                {
                    i++;
                }
            }

            // move past the closing bracket character
            i++;

            //assign field value
            if (document[fieldName] == null)
            {
                document[fieldName] = RawDocument.Substring(startIndex, i - startIndex);
            }
            else
            {
                ((List<object>)document[fieldName]).Add(RawDocument.Substring(startIndex, i - startIndex));
            }

            return i;
        }

        private int ParseValue(int i, Dictionary<string, object> document, string fieldName)
        {
            int startIndex = i;

            // search for end of parsed field value
            while ((i < RawDocument.Length) && (RawDocument[i] != ',') && (RawDocument[i] != ')') && (RawDocument[i] != ']'))
            {
                i++;
            }

            // determine the type of field value

            string stringValue = RawDocument.Substring(startIndex, i - startIndex);
            object value = new object();

            if (stringValue.Length > 0)
            {
                // binary content
                if ((stringValue.Length > 2) && (stringValue[0] == '_') && (stringValue[stringValue.Length - 1] == '_'))
                {
                    stringValue = stringValue.Substring(1, stringValue.Length - 2);

                    // need to be able for base64 encoding which requires content to be devidable by 4
                    int mod4 = stringValue.Length % 4;

                    if (mod4 > 0)
                    {
                        stringValue += new string('=', 4 - mod4);
                    }

                    value = Convert.FromBase64String(stringValue);
                }
                // datetime or date
                else if ((stringValue.Length > 2) && (stringValue[stringValue.Length - 1] == 't') || (stringValue[stringValue.Length - 1] == 'a'))
                {
                    // Unix timestamp is miliseconds past epoch
                    DateTime epoch = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
                    string foo = stringValue.Substring(0, stringValue.Length - 1);
                    double d = double.Parse(foo);
                    value = epoch.AddMilliseconds(d).ToUniversalTime();
                }
                // boolean
                else if ((stringValue.Length > 2) && (stringValue == "true") || (stringValue == "false"))
                {
                    value = (stringValue == "true") ? true : false;
                }
                // numbers
                else
                {
                    char lastCharacter = stringValue[stringValue.Length - 1];

                    switch (lastCharacter)
                    {
                        case 'b':
                            value = byte.Parse(stringValue.Substring(0, stringValue.Length - 1));
                            break;
                        case 's':
                            value = short.Parse(stringValue.Substring(0, stringValue.Length - 1));
                            break;
                        case 'l':
                            value = long.Parse(stringValue.Substring(0, stringValue.Length - 1));
                            break;
                        case 'f':
                            value = float.Parse(stringValue.Substring(0, stringValue.Length - 1), CultureInfo.InvariantCulture);
                            break;
                        case 'd':
                            value = double.Parse(stringValue.Substring(0, stringValue.Length - 1), CultureInfo.InvariantCulture);
                            break;
                        case 'c':
                            value = decimal.Parse(stringValue.Substring(0, stringValue.Length - 1), CultureInfo.InvariantCulture);
                            break;
                        default:
                            value = int.Parse(stringValue);
                            break;
                    }
                }
            }
            // null
            else if (stringValue.Length == 0)
            {
                value = null;
            }

            //assign field value
            if (document[fieldName] == null)
            {
                document[fieldName] = value;
            }
            else
            {
                ((List<object>)document[fieldName]).Add(value);
            }

            return i;
        }

        private int ParseEmbeddedDocument(int i, Dictionary<string, object> document, string fieldName)
        {
            // move to the inside of embedded document (go past starting bracket character)
            i++;

            // create new dictionary which would hold K/V pairs of embedded document
            Dictionary<string, object> embeddedDocument = new Dictionary<string, object>();

            if (document[fieldName] == null)
            {
                document[fieldName] = embeddedDocument;
            }
            else
            {
                ((List<object>)document[fieldName]).Add(embeddedDocument);
            }

            // start parsing field names until the closing bracket of embedded document is reached
            while (RawDocument[i] != ')')
            {
                i = ParseFieldName(i, embeddedDocument);
            }

            // move past close bracket of embedded document
            i++;

            return i;
        }

        private int ParseCollection(int i, Dictionary<string, object> document, string fieldName)
        {
            // move to the first element of this collection
            i++;

            document[fieldName] = new List<object>();

            while (RawDocument[i] != ']')
            {
                // check what follows after parsed field name and start parsing underlying type
                switch (RawDocument[i])
                {
                    case '"':
                        i = ParseString(i, document, fieldName);
                        break;
                    case '#':
                        i = ParseRecordID(i, document, fieldName);
                        break;
                    case '(':
                        i = ParseEmbeddedDocument(i, document, fieldName);
                        break;
                    case '{':
                        i = ParseMap(i, document, fieldName);
                        break;
                    case ',':
                        i++;
                        break;
                    default:
                        i = ParseValue(i, document, fieldName);
                        break;
                }
            }

            // move past close bracket of collection
            i++;

            return i;
        }
    }
}
