using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;

namespace Sandwych.Reporting.OfficeML
{
    public class WordMLDocument : AbstractXmlDocument<WordMLDocument>
    {
        public const string DestAttribute = "w:dest";
        public const string HlinkElement = "w:hlink";
        public const string BookMarkElement = "w:bookmark";

        public override XmlNamespaceManager CreateXmlNamespaceManager(XmlDocument xmlDoc) =>
            new WordMLNamespaceManager(xmlDoc.NameTable);
    }
}
