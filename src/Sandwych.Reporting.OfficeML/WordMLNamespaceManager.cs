//作者：李维
//创建时间：2010-09-09
using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;

namespace Sandwych.Reporting.OfficeML
{
    internal sealed class WordMLNamespaceManager : XmlNamespaceManager
    {
        public const string WNamespace = "http://schemas.microsoft.com/office/word/2003/wordml";
        public const string ONamespace = "urn:schemas-microsoft-com:office:office";
        public const string VNamespace = "urn:schemas-microsoft-com:vml";

        public WordMLNamespaceManager(XmlNameTable xnt)
            : base(xnt)
        {
            this.LoadNamespaces();
        }

        private void LoadNamespaces()
        {
            this.AddNamespace("w", WNamespace);
            this.AddNamespace("o", ONamespace);
            this.AddNamespace("v", VNamespace);
        }
    }
}
