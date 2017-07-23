using System.Xml;

namespace Sandwych.Reporting.OpenDocument
{
    public sealed class OdfNamespaceManager : XmlNamespaceManager
    {
        public const string ManifestNamespace = @"urn:oasis:names:tc:opendocument:xmlns:manifest:1.0";

        public OdfNamespaceManager(XmlNameTable xnt)
            : base(xnt)
        {
            this.LoadNamespaces();
        }

        private void LoadNamespaces()
        {
            this.AddNamespace("manifest", ManifestNamespace);
            this.AddNamespace("text", @"urn:oasis:names:tc:opendocument:xmlns:text:1.0");
            this.AddNamespace("table", @"urn:oasis:names:tc:opendocument:xmlns:table:1.0");
            this.AddNamespace("xlink", @"http://www.w3.org/1999/xlink");
            this.AddNamespace("draw", @"urn:oasis:names:tc:opendocument:xmlns:drawing:1.0");
            this.AddNamespace("office", @"urn:oasis:names:tc:opendocument:xmlns:office:1.0");
            this.AddNamespace("calcext", @"urn:org:documentfoundation:names:experimental:calc:xmlns:calcext:1.0");
        }
    }
}