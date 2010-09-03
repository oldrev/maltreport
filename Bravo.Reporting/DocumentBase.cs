//2010-09-02

using System;
using System.Collections.Generic;
using System.Collections;
using System.Text;
using System.IO;
using System.Xml;
using System.Diagnostics;


namespace Bravo.Reporting
{
    public abstract class DocumentBase : IDocument
    {

        #region ITemplate 成员

        public abstract void Save(Stream outStream);
        public abstract void Load(Stream inStream);

        public virtual void Save(string path)
        {
            if (string.IsNullOrEmpty(path))
            {
                throw new ArgumentNullException("path");
            }

            using (var fs = File.Open(path, FileMode.CreateNew, FileAccess.Write))
            {
                this.Save(fs);
            }
        }

        public virtual void Load(string path)
        {
            if (string.IsNullOrEmpty(path))
            {
                throw new ArgumentNullException("path");
            }

            using (var fs = File.Open(path, FileMode.Open, FileAccess.Read))
            {
                this.Load(fs);
            }
        }

        public abstract Stream GetEntryInputStream(string entryPath);

        public abstract Stream GetEntryOutputStream(string entryPath);

        public abstract string MainContentEntryPath { get; }

        #endregion

        public virtual byte[] GetBuffer()
        {
            using (var ms = new MemoryStream())
            {
                this.Save(ms);
                return ms.GetBuffer();
            }
        }

        public string ToBase64String()
        {
            return Convert.ToBase64String(this.GetBuffer());
        }


        public TextReader GetEntryTextReader(string entryPath)
        {
            if (string.IsNullOrEmpty(entryPath))
            {
                throw new ArgumentNullException("entryPath");
            }

            return new StreamReader(this.GetEntryInputStream(entryPath));
        }

        public TextWriter GetEntryTextWriter(string entryPath)
        {
            if (string.IsNullOrEmpty(entryPath))
            {
                throw new ArgumentNullException("entryPath");
            }

            return new StreamWriter(this.GetEntryOutputStream(entryPath));
        }

        public void WriteXmlEntry(XmlDocument xml, string entryPath)
        {
            if (xml == null)
            {
                throw new ArgumentNullException("xml");
            }

            if (string.IsNullOrEmpty(entryPath))
            {
                throw new ArgumentNullException("entryPath");
            }

            using (var cos = this.GetEntryOutputStream(entryPath))
            using (var writer = new XmlTextWriter(cos, Encoding.UTF8))
            {
                writer.Formatting = Formatting.Indented; //对于 Velocity 模板，最好格式化
                xml.WriteTo(writer);
            }
        }

        public XmlDocument ReadXmlEntry(string entryPath)
        {
            var xml = new XmlDocument();
            using (var contentStream = this.GetEntryInputStream(entryPath))
            {
                xml.Load(contentStream);
            }
            return xml;
        }

        public void WriteMainContentXml(XmlDocument xml)
        {
            this.WriteXmlEntry(xml, this.MainContentEntryPath);
        }

        public XmlDocument ReadMainContentXml()
        {
            return this.ReadXmlEntry(this.MainContentEntryPath);
        }

        public abstract ICollection<string> EntryPaths { get; }

        public abstract bool EntryExists(string entryPath);

        public abstract string AddImage(Image img);

        protected static void CopyStream(Stream src, Stream dest)
        {
            if (src == null)
            {
                throw new ArgumentNullException("src");
            }

            if (dest == null)
            {
                throw new ArgumentNullException("dest");
            }

            var bufSize = 2048;
            var buf = new byte[bufSize];
            int nRead = 0;
            while ((nRead = src.Read(buf, 0, bufSize)) > 0)
            {
                dest.Write(buf, 0, nRead);
            }
        }

        public virtual void CopyTo(IDocument destDoc)
        {
            foreach (var item in this.EntryPaths)
            {
                using (var inStream = this.GetEntryInputStream(item))
                using (var outStream = destDoc.GetEntryOutputStream(item))
                {
                    CopyStream(inStream, outStream);
                }
            }
        }
    }
}
