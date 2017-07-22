using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml;

namespace Sandwych.Reporting.OpenDocument
{
    public abstract class OdfXmlDocument : XmlDocument
    {
        private readonly OdfNamespaceManager _nsmanager;

        public OdfXmlDocument(Stream stream) : base()
        {
            this.Load(stream);
            _nsmanager = new OdfNamespaceManager(this.NameTable);
            _nsmanager.LoadOpenDocumentNamespaces();
        }

        public OdfNamespaceManager NamespaceManager => _nsmanager;

    }
}
