using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;

namespace Sandwych.Reporting.OfficeML
{
    internal sealed class ExcelMLNamespaceManager : XmlNamespaceManager
    {
        public const string WorkbookNamespace = @"urn:schemas-microsoft-com:office:spreadsheet";
        public const string ONamespace = @"urn:schemas-microsoft-com:office:office";
        public const string XNamespace = @"urn:schemas-microsoft-com:office:excel";
        public const string SSNamespace = @"urn:schemas-microsoft-com:office:spreadsheet";
        public const string HtmlNamespace = @"http://www.w3.org/TR/REC-html40";
        public const string ExcelWorkbookNamespace = @"urn:schemas-microsoft-com:office:excel";

        public ExcelMLNamespaceManager(XmlNameTable xnt)
            : base(xnt)
        {
            this.LoadNamespaces();
        }

        private void LoadNamespaces()
        {
            this.AddNamespace("o", ONamespace);
            this.AddNamespace("x", XNamespace);
            this.AddNamespace("ss", SSNamespace);
            this.AddNamespace("html", HtmlNamespace);
        }
    }
}
