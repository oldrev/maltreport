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
        public const string DrawFrameElement = @"draw:frame";
        public const string TextAnchorElement = @"text:a";
        public const string TextPlaceholderTypeAttribute = @"text:placeholder-type";
        public const string TableRowElement = @"table:table-row";

        public readonly Lazy<List<OdfBlobEntry>> _blobs = new Lazy<List<OdfBlobEntry>>(() => new List<OdfBlobEntry>(), true);

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
                await this.AddZipEntryAsync(zip, MimeTypeEntryPath);
                this.Entries.Remove(MimeTypeEntryPath);

                foreach (var item in this.Entries)
                {
                    await this.AddZipEntryAsync(zip, item.Key);
                }
            }
        }

        public override void Save(Stream outStream) =>
            this.SaveAsync(outStream).GetAwaiter().GetResult();

        public OdfBlobEntry AddOrGetImage(Blob imageBlob)
        {
            if (imageBlob == null)
            {
                throw new ArgumentNullException(nameof(imageBlob));
            }

            var fullPath = "Pictures/" + imageBlob.FileName;

            var existedBlob = _blobs.Value.FirstOrDefault(b => b.Blob.Id == imageBlob.Id);

            if (existedBlob != null)
            {
                return existedBlob;
            }

            using (var outStream = this.GetEntryOutputStream(fullPath))
            {
                outStream.Write(imageBlob.GetBuffer(), 0, imageBlob.Length);
            }

            var manifestDoc = new OdfManifestDocument();
            using (var manifestInStream = this.GetEntryInputStream(OdfDocument.ManifestEntryPath))
            {
                manifestDoc.Load(manifestInStream);
            }

            //manifestDoc.CreatePicturesEntryElement();
            manifestDoc.AppendImageFileEntry(imageBlob.ExtensionName, fullPath);

            using (var manifestOutStream = this.GetEntryOutputStream(OdfDocument.ManifestEntryPath))
            {
                manifestDoc.Save(manifestOutStream);
            }

            var blobEntry = new OdfBlobEntry(fullPath, imageBlob);
            this._blobs.Value.Add(blobEntry);
            return blobEntry;
        }

        public IEnumerable<OdfBlobEntry> BlobEntries => _blobs.Value;

        public OdfDocument Clone()
        {
            var destDoc = new OdfDocument();
            this.SaveAs(destDoc);
            return destDoc;
        }

        public void WriteMainContentXml(XmlDocument xml) =>
            this.WriteXmlEntry(this.MainContentEntryPath, xml);

        public XmlDocument ReadMainContentXml() =>
            this.ReadXmlEntry(this.MainContentEntryPath);

    }
}
