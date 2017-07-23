using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace Sandwych.Reporting
{
    public abstract class AbstractXmlDocument : IDocument
    {
        private XmlNamespaceManager _nsManager = null;
        private readonly XmlDocument _xmlDocument;

        public XmlNamespaceManager NamespaceManager => _nsManager;
        public XmlDocument XmlDocument => _xmlDocument;

        public AbstractXmlDocument()
        {
            _xmlDocument = new XmlDocument();
        }

        public abstract XmlNamespaceManager CreateXmlNamespaceManager(XmlDocument xmlDoc);

        public byte[] AsBuffer()
        {
            using (var ms = new MemoryStream())
            {
                _xmlDocument.Save(ms);
                return ms.ToArray();
            }
        }

        public virtual void Load(Stream inStream)
        {
            _xmlDocument.Load(inStream);
            _nsManager = this.CreateXmlNamespaceManager(_xmlDocument);
        }

        public virtual Task LoadAsync(Stream inStream)
        {
            return Task.Factory.StartNew(() => this.Load(inStream));
        }

        public virtual void Save(Stream outStream)
        {
            _xmlDocument.Save(outStream);
        }

        public virtual Task SaveAsync(Stream outStream)
        {
            return Task.Factory.StartNew(() => this.Save(outStream));
        }
    }
}
