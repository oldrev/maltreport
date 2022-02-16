using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Xml;

namespace Sandwych.Reporting.OfficeML
{

    public abstract class AbstractXmlDocument<TDocument> : AbstractDocument<TDocument>
        where TDocument : AbstractXmlDocument<TDocument>, new()
    {
        private XmlNamespaceManager _nsManager = null;
        private readonly XmlDocument _xmlDocument;

        public XmlNamespaceManager NamespaceManager => _nsManager;
        public XmlDocument XmlDocument => _xmlDocument;

        protected AbstractXmlDocument()
        {
            _xmlDocument = new XmlDocument();
        }

        public abstract XmlNamespaceManager CreateXmlNamespaceManager(XmlDocument xmlDoc);

        public override async Task LoadAsync(Stream inStream, CancellationToken ct = default)
        {
            await Task.Factory.StartNew(() =>
            {
                _xmlDocument.Load(inStream);
                _nsManager = this.CreateXmlNamespaceManager(_xmlDocument);
                this.OnLoaded();
            }, ct);
        }

        public override async Task SaveAsync(Stream outStream, CancellationToken ct = default)
        {
            await Task.Factory.StartNew(() => _xmlDocument.Save(outStream), ct);
        }

        public static TDocument LoadFromText(string xml)
        {
            var doc = new TDocument();
            doc.XmlDocument.LoadXml(xml);
            doc._nsManager = doc.CreateXmlNamespaceManager(doc._xmlDocument);
            return doc;
        }
    }
}
