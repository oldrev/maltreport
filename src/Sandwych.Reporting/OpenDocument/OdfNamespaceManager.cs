using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;

namespace Sandwych.Reporting.OpenDocument
{
    public sealed class OdfNamespaceManager : XmlNamespaceManager
    {
        public const string ManifestNamespace = @"urn:oasis:names:tc:opendocument:xmlns:manifest:1.0";

        public OdfNamespaceManager(XmlNameTable xnt)
            : base(xnt)
        {
        }

        public void LoadOpenDocumentNamespaces()
        {
            this.AddNamespace("manifest", ManifestNamespace);
            this.AddNamespace("text", @"urn:oasis:names:tc:opendocument:xmlns:text:1.0");
            this.AddNamespace("table", @"urn:oasis:names:tc:opendocument:xmlns:table:1.0");
            this.AddNamespace("xlink", @"http://www.w3.org/1999/xlink");
            this.AddNamespace("draw", @"urn:oasis:names:tc:opendocument:xmlns:drawing:1.0");
        }

    }
}
