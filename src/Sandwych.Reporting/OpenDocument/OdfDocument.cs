using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Threading.Tasks;
using Sandwych.Reporting.OpenDocument.Xml;

namespace Sandwych.Reporting.OpenDocument
{
    public class OdfDocument : AbstractZipDocument<OdfDocument>
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

        public readonly Lazy<List<DocumentBlobEntry>> _blobs = new Lazy<List<DocumentBlobEntry>>(() => new List<DocumentBlobEntry>(), true);
        private readonly Lazy<OdfManifestXmlDocument> _manifestDocument;
        public string MainContentEntryPath => ContentEntryPath;

        public OdfManifestXmlDocument ManifestXmlDocument => _manifestDocument.Value;

        public OdfDocument()
        {
            _manifestDocument = new Lazy<OdfManifestXmlDocument>(this.LoadManifestDocument, true);
        }

        public void Flush()
        {
            //Before save to other doc, we must save manifest
            if (_manifestDocument.IsValueCreated)
            {
                using (var s = this.OpenOrCreateEntryToWrite(ManifestEntryPath))
                {
                    _manifestDocument.Value.Save(s);
                }
            }
        }

        private OdfManifestXmlDocument LoadManifestDocument()
        {
            using (var s = this.OpenEntryToRead(ManifestEntryPath))
            {
                return new OdfManifestXmlDocument(s);
            }
        }

        public override async Task LoadAsync(Stream inStream)
        {
            await base.LoadAsync(inStream);
        }

        public override async Task SaveAsync(Stream outStream)
        {
            this.Flush();

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
            Task.Run(() => this.SaveAsync(outStream)).Wait();

        public DocumentBlobEntry AddOrGetImage(Blob imageBlob)
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

            this.SetEntryBuffer(fullPath, imageBlob.GetBuffer());

            _manifestDocument.Value.AppendImageFileEntry(imageBlob.ExtensionName, fullPath);

            var blobEntry = new DocumentBlobEntry(fullPath, imageBlob);
            this._blobs.Value.Add(blobEntry);
            return blobEntry;
        }

        public IEnumerable<DocumentBlobEntry> BlobEntries => _blobs.Value;

        public override void SaveAs(OdfDocument destDoc)
        {
            this.Flush();

            base.SaveAs(destDoc);
        }

        public void WriteMainContentXml(OdfContentXmlDocument xml) =>
            this.WriteXmlEntry(this.MainContentEntryPath, xml);

        public OdfContentXmlDocument ReadMainContentXml()
        {
            using (var s = this.OpenEntryToRead(MainContentEntryPath))
            {
                var doc = new OdfContentXmlDocument(s);
                return doc;
            }
        }
    } //class
} //namespace