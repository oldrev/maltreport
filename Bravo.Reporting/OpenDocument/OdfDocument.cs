//作者：李维
//创建时间：2010-08-20
using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Diagnostics;
using System.Xml;

using Ionic.Zip;

namespace Bravo.Reporting.OpenDocument
{
    public class OdfDocument : ZipArchiveDocumentBase
    {
        internal const string MimeTypeEntryPath = "mimetype";
        internal const string SettingsEntryPath = "settings.xml";

        public const string ManifestEntryPath = "META-INF/manifest.xml";

        /// <summary>
        /// 加载到内存的 ODF 的文件内容
        /// </summary>
        private IDictionary<string, byte[]> entries = new Dictionary<string, byte[]>();

        public OdfDocument()
        {
        }

        public override void Load(Stream inStream)
        {
            if (inStream == null)
            {
                throw new ArgumentNullException("inStream");
            }

            this.entries.Clear();

            //把 zip 的内容加载到内存
            using (var zf = ZipFile.Read(inStream))
            {
                foreach (ZipEntry ze in zf)
                {
                    using (var ws = this.GetEntryOutputStream(ze.FileName))
                    {
                        ze.Extract(ws);
                    }
                }
            }
        }

        public override void Save(Stream outStream)
        {
            if (outStream == null || !outStream.CanWrite)
            {
                throw new ArgumentNullException("outStream");
            }

            //ODF 格式约定 mimetype 必须为第一个文件
            if (!this.entries.ContainsKey(MimeTypeEntryPath))
            {
                throw new InvalidDataException("Can not found entry: 'mimetype'");
            }

            using (var ze = new ZipFile())
            {
                this.AppendZipEntry(ze, MimeTypeEntryPath);

                foreach (var item in this.entries)
                {
                    if (item.Key == MimeTypeEntryPath)
                    {
                        continue;
                    }

                    this.AppendZipEntry(ze, item.Key);
                }

                ze.Save(outStream);
            }
        }

        private void AppendZipEntry(ZipFile zf, string name)
        {
            Debug.Assert(zf != null);
            Debug.Assert(!string.IsNullOrEmpty(name));
            Debug.Assert(this.entries.ContainsKey(name));

            var data = this.entries[name];
            var ze = zf.AddEntry(name, data);

            var extensionName = Path.GetExtension(name).ToUpperInvariant();

            switch (extensionName)
            {
                case "JPEG":
                case "JPG":
                case "PNG":
                    ze.CompressionMethod = CompressionMethod.None;
                    break;

                default:
                    ze.CompressionMethod = CompressionMethod.Deflate;
                    ze.CompressionLevel = Ionic.Zlib.CompressionLevel.BestCompression;
                    break;
            }
        }

        public override ICollection<string> EntryPaths
        {
            get { return this.entries.Keys; }
        }

        public override Stream GetEntryInputStream(string entryPath)
        {
            if (string.IsNullOrEmpty(entryPath))
            {
                throw new ArgumentNullException("entryPath");
            }

            var data = this.entries[entryPath];
            return new MemoryStream(data);
        }

        /// <summary>
        /// 千万要记得关闭
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public override Stream GetEntryOutputStream(string entryPath)
        {
            if (string.IsNullOrEmpty(entryPath))
            {
                throw new ArgumentNullException("entryPath");
            }

            var oms = new OutputMemoryStream(entryPath, this.entries);
            this.entries[entryPath] = oms.GetBuffer();

            return oms;
        }

        public override bool EntryExists(string entryPath)
        {
            if (string.IsNullOrEmpty(entryPath))
            {
                throw new ArgumentNullException("entryPath");
            }
            return this.entries.ContainsKey(entryPath);
        }

        public override string AddImage(Image img)
        {
            if (img == null)
            {
                throw new ArgumentNullException("img");
            }

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

        public override string MainContentEntryPath { get { return "content.xml"; } }

        public override ITemplate Compile()
        {
            return OdfCompiler.Compile(this);
        }

        #region ICloneable 接口
        public override object Clone()
        {
            var destDoc = new OdfDocument();
            this.CopyTo(destDoc);
            return destDoc;
        }
        #endregion
    }
}
