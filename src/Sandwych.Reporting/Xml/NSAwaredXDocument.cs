using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;
using System.Xml.XPath;

namespace Sandwych.Reporting.Xml
{
    public class NSAwaredXDocument : XDocument
    {
        public XmlNamespaceManager NSManager { get; } = new XmlNamespaceManager(new NameTable());

        private NSAwaredXDocument(XDocument xdoc) : base(xdoc)
        {
            var xmlNameSpaceList = (xdoc.XPathEvaluate(@"//namespace::*[not(. = ../../namespace::*)]") as IEnumerable).Cast<XAttribute>();
            foreach (var nsNode in xmlNameSpaceList)
            {
                NSManager.AddNamespace(nsNode.Name.LocalName, nsNode.Value);
            }
        }

        public static async Task<NSAwaredXDocument> LoadFromAsync(Stream inStream, CancellationToken ct = default)
        {
#if NET5_0_OR_GREATER || NETCOREAPP3_1_OR_GREATER || NETSTANDARD2_1_OR_GREATER
            return new NSAwaredXDocument(await XDocument.LoadAsync(inStream, LoadOptions.None, ct));
#else
            var xdoc = await Task.Factory.StartNew(() => XDocument.Load(inStream, LoadOptions.None))
                .ConfigureAwait(false);
            return new NSAwaredXDocument(xdoc);
#endif
        }

        public IEnumerable<XElement> NSAwaredXPathSelectElements(string xpath) =>
            this.Document.XPathSelectElements(xpath, this.NSManager);

        public XElement NSAwaredXPathSelectElement(string xpath) =>
            this.Document.XPathSelectElement(xpath, this.NSManager);

        public XName GetFullName(string ns, string name) => this.GetNamespace(ns) + name;

        public XNamespace GetNamespace(string ns) => XNamespace.Get(this.NSManager.LookupNamespace(ns));
    }
}
