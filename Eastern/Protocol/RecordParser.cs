
namespace Eastern.Protocol
{
    internal static class RecordParser
    {
        internal static ODocument ToDocument(int version, string rawDocument)
        {
            ODocument document = new ODocument();
            document.Version = version;

            int atIndex = rawDocument.IndexOf('@');
            int colonIndex = rawDocument.IndexOf(':');

            if ((atIndex != -1) && (atIndex < colonIndex))
            {
                document.Class = rawDocument.Substring(0, atIndex);
            }

            return document;
        }
    }
}
