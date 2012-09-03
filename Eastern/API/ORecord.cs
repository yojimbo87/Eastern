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

            // check what follow after parsed field name and start parsing underlying type
            switch (RawDocument[i])
            {
                case '"':
                    i = ParseString(i, currentDocument, fieldName);
                    break;
                case '#':
                    i = ParseRecordID(i++, currentDocument, fieldName);
                    break;
                case '(':
                    i = ParseEmbeddedDocument(i, currentDocument, fieldName);
                    break;
                /*case '[':
                    i = ParseCollection(i);
                    break;*/
                default:
                    break;
            }

            // single string value was parsed and we need to push the index if next character is comma
            if (RawDocument[i] == ',')
            {
                i++;
            }

            return i;
        }

        private int ParseString(int i, Dictionary<string, object> fields, string fieldName)
        {
            // move to the inside of string
            i++;

            int startIndex = i;

            // search for end of the parsed string value
            while (RawDocument[i] != '"')
            {
                i++;
            }

            // assign field value
            fields[fieldName] = RawDocument.Substring(startIndex, i - startIndex);
            // move past the closing quote character
            i++;

            return i;
        }

        private int ParseRecordID(int i, Dictionary<string, object> fields, string fieldName)
        {
            int startIndex = i;

            // search for end of parsed record ID value
            while ((RawDocument[i] != ',') && (RawDocument[i] != ')') && (i < RawDocument.Length))
            {
                i++;
            }

            //assign field value
            fields[fieldName] = RawDocument.Substring(startIndex, i - startIndex);

            return i;
        }

        private int ParseEmbeddedDocument(int i, Dictionary<string, object> fields, string fieldName)
        {
            // move to the inside of embedded document (go past starting bracket character)
            i++;

            int startIndex = i;

            // create new dictionary which would hold K/V pairs of embedded document
            Dictionary<string, object> embeddedDocument = new Dictionary<string, object>();
            fields[fieldName] = embeddedDocument;

            // start parsing field names until the closing bracket of embedded document is reached
            while (RawDocument[i] != ')')
            {
                i = ParseFieldName(i, embeddedDocument);
            }

            // move past close bracket of embedded document
            i++;

            return i;
        }
    }
}
