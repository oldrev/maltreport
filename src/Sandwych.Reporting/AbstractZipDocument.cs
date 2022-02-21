using Sandwych.Reporting.IO;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Xml;

namespace Sandwych.Reporting
{
    public abstract class AbstractZipDocument<TDocument> : AbstractDocument<TDocument>, IZipDocument
        where TDocument : AbstractZipDocument<TDocument>, new()
    {
        private readonly IDictionary<string, byte[]> _documentEntries = new Dictionary<string, byte[]>();

        public IDictionary<string, byte[]> Entries => _documentEntries;

        public override async Task LoadAsync(Stream inStream, CancellationToken ct = default)
        {
            if (inStream == null)
            {
                throw new ArgumentNullException(nameof(inStream));
            }

            if (_documentEntries.Count > 0)
            {
                _documentEntries.Clear();
            }

            // Load zipped content into the memory
            using var archive = new ZipArchive(inStream, ZipArchiveMode.Read, leaveOpen: true);
            foreach (ZipArchiveEntry ze in archive.Entries)
            {
                using var zs = ze.Open();
                var buf = await zs.ReadAllBytesAsync(ct);
                if (buf.Length != ze.Length)
                {
                    throw new IOException("Failed to read zip entry: " + ze.FullName);
                }
                _documentEntries[ze.FullName] = buf;
            }

            this.OnLoaded();
        }

        public override async Task SaveAsync(Stream outStream, CancellationToken ct = default)
        {
            using var zip = new ZipArchive(outStream, ZipArchiveMode.Create, leaveOpen: true);
            foreach (var item in _documentEntries)
            {
                await this.AddZipEntryAsync(zip, item.Key);
            }
        }

        protected async Task AddZipEntryAsync(ZipArchive archive, string name, CancellationToken ct = default)
        {
            Debug.Assert(archive != null);
            Debug.Assert(!string.IsNullOrEmpty(name));
            Debug.Assert(this._documentEntries.ContainsKey(name));

            var data = this._documentEntries[name];

            var extensionName = Path.GetExtension(name).ToLowerInvariant();
            var cl = CompressionLevel.Fastest;
            switch (extensionName)
            {
                case ".zip":
                case ".jpeg":
                case ".jpg":
                case ".png":
                case ".gif":
                case ".mp3":
                case ".avi":
                case ".mp4":
                    cl = CompressionLevel.NoCompression;
                    break;

                default:
                    cl = CompressionLevel.Optimal;
                    break;
            }
            var zae = archive.CreateEntry(name, cl);
            using var zs = zae.Open();
            await zs.WriteAsync(data, 0, data.Length, ct);
        }

        public IEnumerable<string> EntryPaths
        {
            get { return this._documentEntries.Keys; }
        }

        public byte[] GetEntryBuffer(string entryPath) => _documentEntries[entryPath];

        public void SetEntryBuffer(string entryPath, byte[] buffer) => _documentEntries[entryPath] = buffer;

        public async Task SetEntryAsync(string entryPath, Stream inStream)
        {
            var buf = await inStream.ReadAllBytesAsync();
            this.SetEntryBuffer(entryPath, buf);
        }

        public bool EntryExists(string entryPath)
        {
            if (string.IsNullOrEmpty(entryPath))
            {
                throw new ArgumentNullException(nameof(entryPath));
            }
            return this._documentEntries.ContainsKey(entryPath);
        }

        protected static void CopyStream(Stream src, Stream dest) =>
            Task.Run(() => CopyStreamAsync(src, dest)).Wait();

        protected static async Task CopyStreamAsync(Stream src, Stream dest, CancellationToken ct = default)
        {
            if (src == null)
            {
                throw new ArgumentNullException("src");
            }

            if (dest == null)
            {
                throw new ArgumentNullException("dest");
            }

            var bufSize = 1024 * 8;
            var buf = new byte[bufSize];
            int nRead = 0;
            while ((nRead = await src.ReadAsync(buf, 0, bufSize)) > 0)
            {
                if (ct != default && ct.IsCancellationRequested)
                {
                    ct.ThrowIfCancellationRequested();
                }
                await dest.WriteAsync(buf, 0, nRead, ct);
            }
        }

        public Stream OpenEntryToRead(string entryPath)
        {
            if(!this.EntryExists(entryPath))
            {
                throw new FileNotFoundException(entryPath);
            }
            return new MemoryStream(this.GetEntryBuffer(entryPath));
        }

        public Stream OpenOrCreateEntryToWrite(string entryPath)
        {
            var oms = new ZipDocumentEntryOutputMemoryStream<TDocument>(entryPath, this);
            return oms;
        }

    }
}