using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;

namespace Sandwych.Reporting.OfficeML
{
    public class ExcelMLDocument : AbstractXmlDocument<ExcelMLDocument>
    {
        public const string IndexAttribute = "ss:Index";
        public const string ExpandedColumnCountAttribute = "ss:ExpandedColumnCount";
        public const string ExpandedRowCountAttribute = "ss:ExpandedRowCount";
        public const string FormatAttribute = "ss:Format";
        public const string TypeAttribute = "ss:Type";
        public const string HRefAttribute = "ss:HRef";

        public override XmlNamespaceManager CreateXmlNamespaceManager(XmlDocument xmlDoc) =>
            new ExcelMLNamespaceManager(xmlDoc.NameTable);
    }
}
