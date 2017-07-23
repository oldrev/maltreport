using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;

namespace Sandwych.Reporting.OfficeML
{
    public class WordMLDocument : AbstractXmlDocument
    {
        public override XmlNamespaceManager CreateXmlNamespaceManager(XmlDocument xmlDoc) =>
            new WordMLNamespaceManager(xmlDoc.NameTable);
    }
}
