//作者：李维
//创建时间：2010-08-20
using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Diagnostics;
using System.Xml;

using ICSharpCode.SharpZipLib.Zip;

namespace Bravo.Reporting.OpenDocument
{
    public class OdfDocument : ITemplate, ICompiledTemplate
    {
        public const string MimeTypeEntryPath = "mimetype";
        public const string ContentEntryPath = "content.xml";
        public const string ManifestEntryPath = "META-INF/manifest.xml";
        public const string SettingsEntryPath = "settings.xml";

        /// <summary>
        /// 加载到内存的 ODF 的文件内容
        /// </summary>
        private IDictionary<string, byte[]> odfEntries = new Dictionary<string, byte[]>();

        public OdfDocument()
        {
            this.OdfPath = null;
        }

        public OdfDocument(string odfPath)
        {
            if (string.IsNullOrEmpty(odfPath))
            {
                throw new ArgumentNullException("odfPath");
            }

            this.OdfPath = odfPath;

            this.LoadContents(odfPath);
        }

        public OdfDocument(Stream documentStream)
        {
            if (documentStream == null)
            {
                throw new ArgumentNullException("documentStream");
            }

            this.LoadContents(documentStream);
        }

        private void LoadContents(string path)
        {
            Debug.Assert(!string.IsNullOrEmpty(path));

            using (var fs = File.OpenRead(path))
            {
                this.LoadContents(fs);
            }
        }

        private void LoadContents(Stream stream)
        {
            Debug.Assert(stream != null);

            //把 zip 的内容加载到内存
            using (var zf = new ZipFile(this.OdfPath))
            {
                foreach (ZipEntry ze in zf)
                {
                    var zis = zf.GetInputStream(ze.ZipFileIndex);

                    using (var ws = this.GetEntryOutputStream(ze.Name))
                    {
                        CopyStream(zis, ws);
                    }
                }
            }
        }

        public void Save(Stream outStream)
        {
            if (outStream == null || !outStream.CanWrite)
            {
                throw new ArgumentNullException("outStream");
            }

            //ODF 格式约定 mimetype 必须为第一个文件
            if (!this.odfEntries.ContainsKey(MimeTypeEntryPath))
            {
                throw new InvalidDataException("Can not found entry: 'mimetype'");
            }

            using (var zos = new ZipOutputStream(outStream))
            {
                //zos.SetLevel(0);
                zos.UseZip64 = UseZip64.Off;

                this.WriteZipEntry(zos, MimeTypeEntryPath);

                foreach (var item in this.odfEntries)
                {
                    if (item.Key == MimeTypeEntryPath)
                    {
                        continue;
                    }

                    this.WriteZipEntry(zos, item.Key);
                }
            }
        }

        public void Save(string path)
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

        private void WriteZipEntry(ZipOutputStream zipStream, string name)
        {
            Debug.Assert(zipStream != null);
            Debug.Assert(!string.IsNullOrEmpty(name));
            Debug.Assert(this.odfEntries.ContainsKey(name));

            var data = this.odfEntries[name];
            var ze = new ZipEntry(name);
            zipStream.PutNextEntry(ze);
            zipStream.Write(data, 0, data.Length);
            zipStream.CloseEntry();
        }

        public string OdfPath { get; set; }

        public ICollection<string> OdfEntryNames
        {
            get { return this.odfEntries.Keys; }
        }

        public Stream GetEntryInputStream(string name)
        {
            var data = this.odfEntries[name];
            return new MemoryStream(data);
        }

        public TextReader GetEntryTextReader(string name)
        {
            if (string.IsNullOrEmpty(name))
            {
                throw new ArgumentNullException("name");
            }

            return new StreamReader(this.GetEntryInputStream(name));
        }

        /// <summary>
        /// 千万要记得关闭
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public Stream GetEntryOutputStream(string name)
        {
            if (string.IsNullOrEmpty(name))
            {
                throw new ArgumentNullException("name");
            }

            var oms = new OutputMemoryStream(name, this.odfEntries);
            this.odfEntries[name] = oms.GetBuffer();

            return oms;
        }

        public TextWriter GetEntryTextWriter(string name)
        {
            if (string.IsNullOrEmpty(name))
            {
                throw new ArgumentNullException("name");
            }

            return new StreamWriter(this.GetEntryOutputStream(name));
        }

        public bool EntryExists(string name)
        {
            if (string.IsNullOrEmpty(name))
            {
                throw new ArgumentNullException("name");
            }
            return this.odfEntries.ContainsKey(name);
        }

        public void CopyTo(OdfDocument destDoc)
        {
            if (destDoc == null)
            {
                throw new ArgumentNullException("destDoc");
            }

            foreach (var item in this.odfEntries)
            {
                using (var outStream = destDoc.GetEntryOutputStream(item.Key))
                {
                    outStream.Write(item.Value, 0, item.Value.Length);
                }
            }
        }

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

        public byte[] GetBuffer()
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

        // 下面几个方法是否要提升一级？

        public string AddImage(Image img)
        {
            var fullPath = "Pictures/" + img.DocumentFileName;
            using (var outStream = this.GetEntryOutputStream(fullPath))
            {
                outStream.Write(img.GetData(), 0, img.DataSize);
            }

            var manifestDoc = new OdfManifestDocument();
            using (var manifestInStream = this.GetEntryInputStream(OdfDocument.ManifestEntryPath))
            {
                manifestDoc.Load(manifestInStream);
            }

            manifestDoc.AppendImageFileEntry(img.ExtensionName, fullPath);
            manifestDoc.CreatePicturesEntryElement();

            using (var manifestOutStream = this.GetEntryOutputStream(OdfDocument.ManifestEntryPath))
            {
                manifestDoc.Save(manifestOutStream);
            }

            return fullPath;
        }

        public void WriteXmlEntry(XmlDocument xml, string entryPath)
        {
            if (xml == null)
            {
                throw new ArgumentNullException("xml");
            }

            using (var cos = this.GetEntryOutputStream(entryPath))
            using (var writer = new XmlTextWriter(cos, Encoding.UTF8))
            {
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

        public void WriteContentXml(XmlDocument xml)
        {
            this.WriteXmlEntry(xml, ContentEntryPath);
        }

        public XmlDocument ReadContentXml()
        {
            return this.ReadXmlEntry(ContentEntryPath);
        }
    }
}
