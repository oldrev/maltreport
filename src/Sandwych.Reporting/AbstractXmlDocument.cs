using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace Sandwych.Reporting
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

        public override byte[] AsBuffer()
        {
            using (var ms = new MemoryStream())
            {
                _xmlDocument.Save(ms);
                return ms.ToArray();
            }
        }

        public override void Load(Stream inStream)
        {
            _xmlDocument.Load(inStream);
            _nsManager = this.CreateXmlNamespaceManager(_xmlDocument);
            this.OnLoaded();
        }

        public override async Task LoadAsync(Stream inStream)
        {
            await Task.Factory.StartNew(() => this.Load(inStream));
        }

        public override void Save(Stream outStream)
        {
            _xmlDocument.Save(outStream);
        }

        public override async Task SaveAsync(Stream outStream)
        {
            await Task.Factory.StartNew(() => this.Save(outStream));
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
