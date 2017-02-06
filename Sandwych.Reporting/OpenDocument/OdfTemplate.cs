//作者：李维
//创建时间：2010-08-20
using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Diagnostics;
using System.Xml;
using System.Text.RegularExpressions;
using System.IO.Compression;


using Sandwych.Reporting.Xml;

namespace Sandwych.Reporting.OpenDocument
{
    public class OdfTemplate : AbstractZipBasedTemplate
    {
        internal const string MimeTypeEntryPath = "mimetype";
        internal const string SettingsEntryPath = "settings.xml";
        public const string ManifestEntryPath = "META-INF/manifest.xml";


        /// <summary>
        /// 加载到内存的 ODF 的文件内容
        /// </summary>
        private IDictionary<string, byte[]> entries = new Dictionary<string, byte[]>();

        public OdfTemplate()
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
            using (var archive = new ZipArchive(inStream, ZipArchiveMode.Read))
            {
                foreach (ZipArchiveEntry ze in archive.Entries)
                {
                    using (var zs = ze.Open())
                    {
                        var buf = new byte[ze.Length];
                        var nread = zs.Read(buf, 0, (int)ze.Length);
                        if (nread != ze.Length)
                        {
                            throw new IOException("Failed to read zip entry: " + ze.FullName);
                        }
                        this.entries[ze.FullName] = buf;
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

            //ODF 格式约定 mimetype 必须为 ZIP 包里的第一个文件
            if (!this.entries.ContainsKey(MimeTypeEntryPath))
            {
                throw new InvalidDataException("Entry 'mimetype' not found");
            }

            using (var zip = new ZipArchive(outStream, ZipArchiveMode.Create))
            {
                this.AppendZipEntry(zip, MimeTypeEntryPath);
                this.entries.Remove(MimeTypeEntryPath);

                foreach (var item in this.entries)
                {
                    this.AppendZipEntry(zip, item.Key);
                }
            }
        }

        private void AppendZipEntry(ZipArchive archive, string name)
        {
            Debug.Assert(archive != null);
            Debug.Assert(!string.IsNullOrEmpty(name));
            Debug.Assert(this.entries.ContainsKey(name));

            var data = this.entries[name];

            var extensionName = Path.GetExtension(name).ToUpperInvariant();
            var cl = CompressionLevel.Fastest;
            switch (extensionName)
            {
                case "JPEG":
                case "JPG":
                case "PNG":
                case "MP3":
                case "MP4":
                    cl = CompressionLevel.NoCompression;
                    break;

                default:
                    cl = CompressionLevel.Fastest;
                    break;
            }
            var zae = archive.CreateEntry(name, cl);
            using (var zs = zae.Open())
            {
                zs.Write(data, 0, data.Length);
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
        public override Stream AquireEntryOutputStream(string entryPath)
        {
            if (string.IsNullOrEmpty(entryPath))
            {
                throw new ArgumentNullException("entryPath");
            }
            var oms = new OutputMemoryStream(entryPath, this.entries);
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
            using (var outStream = this.AquireEntryOutputStream(fullPath))
            {
                outStream.Write(img.GetData(), 0, img.DataSize);
            }

            var manifestDoc = new OdfManifestDocument();
            using (var manifestInStream = this.GetEntryInputStream(OdfTemplate.ManifestEntryPath))
            {
                manifestDoc.Load(manifestInStream);
            }

            manifestDoc.AppendImageFileEntry(img.ExtensionName, fullPath);
            manifestDoc.CreatePicturesEntryElement();

            using (var manifestOutStream = this.AquireEntryOutputStream(OdfTemplate.ManifestEntryPath))
            {
                manifestDoc.Save(manifestOutStream);
            }

            return fullPath;
        }

        public override string MainContentEntryPath { get { return "content.xml"; } }

        public override void Compile()
        {
            OdfCompiler.Compile(this);
        }

        #region ICloneable 接口
        public override object Clone()
        {
            var destDoc = new OdfTemplate();
            this.CopyTo(destDoc);
            return destDoc;
        }

        #endregion

        internal void WriteXmlContent(XmlDocument xml)
        {
            //把编译后的 XmlDocument 写入
            using (var cos = this.AquireEntryOutputStream(this.MainContentEntryPath))
            using (var writer = new VelocityEscapedXmlTextWriter(cos))
            {
                writer.Formatting = Formatting.None; //对于 Velocity 模板，最好格式化
                xml.WriteTo(writer);
            }
        }

        private ITextTemplateEngine engine = new VelocityTextTemplateEngine("OdfTemplate");

        private void ResetTextEngine(IDictionary<Image, string> userImages, OdfTemplate resultDocument)
        {
            Debug.Assert(this.engine != null);
            Debug.Assert(userImages != null);
            Debug.Assert(resultDocument != null);

            this.engine.Reset();
            this.engine.RegisterFilter(typeof(string), new XmlStringRenderFilter());
            this.engine.RegisterFilter(typeof(Image), new OdfImageRenderFilter(userImages, resultDocument));
        }

        #region ITemplate 接口实现

        public override IDocument Render(IDictionary<string, object> context)
        {
            Debug.Assert(this.engine != null);

            if (context == null)
            {
                throw new ArgumentNullException("context");
            }

            var resultDoc = new OdfTemplate();
            this.CopyTo(resultDoc);

            var userImages = new Dictionary<Image, string>();

            this.ResetTextEngine(userImages, resultDoc);

            using (var inStream = this.GetEntryInputStream(this.MainContentEntryPath))
            using (var reader = new StreamReader(inStream, Encoding.UTF8))
            using (var ws = resultDoc.AquireEntryOutputStream(resultDoc.MainContentEntryPath))
            using (var writer = new StreamWriter(ws))
            {
                this.engine.Evaluate(context, reader, writer);
            }

            return resultDoc;

        }

        #endregion


    }
}
