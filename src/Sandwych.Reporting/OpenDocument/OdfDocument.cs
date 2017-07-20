using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Linq;
using System.Text;
using System.IO;
using System.Diagnostics;
using System.Xml;
using System.Text.RegularExpressions;
using System.IO.Compression;


using Sandwych.Reporting.Xml;
using System.Threading.Tasks;
using Fluid;

namespace Sandwych.Reporting.OpenDocument
{
    public class OdfDocument : AbstractZippedDocument
    {
        public const string MimeTypeEntryPath = "mimetype";
        public const string SettingsEntryPath = "settings.xml";
        public const string ManifestEntryPath = "META-INF/manifest.xml";
        public const string ContentEntryPath = "content.xml";

        public const string TextPlaceholderElement = @"text:placeholder";
        public const string DrawTextBoxElement = @"draw:text-box";
        public const string TextAnchorElement = @"text:a";
        public const string TextPlaceholderTypeAttribute = @"text:placeholder-type";
        public const string TableRowElement = @"table:table-row";


        public string MainContentEntryPath => ContentEntryPath;

        public OdfDocument()
        {
        }

        public override async Task SaveAsync(Stream outStream)
        {
            //ODF 格式约定 mimetype 必须为 ZIP 包里的第一个文件
            if (!this.Entries.ContainsKey(MimeTypeEntryPath))
            {
                throw new InvalidDataException("Entry 'mimetype' not found");
            }

            using (var zip = new ZipArchive(outStream, ZipArchiveMode.Create))
            {
                await this.AppendZipEntryAsync(zip, MimeTypeEntryPath);
                this.Entries.Remove(MimeTypeEntryPath);

                foreach (var item in this.Entries)
                {
                    await this.AppendZipEntryAsync(zip, item.Key);
                }
            }
        }

        public override void Save(Stream outStream) =>
            this.SaveAsync(outStream).GetAwaiter().GetResult();

        public string AddImage(Image img)
        {
            if (img == null)
            {
                throw new ArgumentNullException(nameof(img));
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



        public OdfDocument Clone()
        {
            var destDoc = new OdfDocument();
            this.CopyTo(destDoc);
            return destDoc;
        }


        public void WriteMainContentXml(XmlDocument xml) =>
            this.WriteXmlEntry(this.MainContentEntryPath, xml);

        public XmlDocument ReadMainContentXml() =>
            this.ReadXmlEntry(this.MainContentEntryPath);

        public void WriteXmlContent(XmlDocument xml)
        {
            //把编译后的 XmlDocument 写入
            using (var cos = this.GetEntryOutputStream(this.MainContentEntryPath))
            using (var writer = XmlWriter.Create(cos))
            {
                xml.WriteTo(writer);
            }
        }

    }
}
