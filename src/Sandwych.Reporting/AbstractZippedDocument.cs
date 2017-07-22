using System;
using System.Collections.Generic;
using System.Collections;
using System.Text;
using System.IO;
using System.Xml;
using System.Diagnostics;
using System.Threading.Tasks;
using Fluid;
using System.IO.Compression;
using Sandwych.Reporting.IO;

namespace Sandwych.Reporting
{
    public abstract class AbstractZippedDocument : IZippedDocument
    {
        private readonly IDictionary<string, byte[]> _documentEntries = new Dictionary<string, byte[]>();

        public IDictionary<string, byte[]> Entries => _documentEntries;

        public virtual void Load(Stream inStream) =>
            this.LoadAsync(inStream).GetAwaiter().GetResult();

        public virtual async Task LoadAsync(Stream inStream)
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
            using (var archive = new ZipArchive(inStream, ZipArchiveMode.Read))
            {
                foreach (ZipArchiveEntry ze in archive.Entries)
                {
                    using (var zs = ze.Open())
                    {
                        var buf = await zs.ReadAllBytesAsync();
                        if (buf.Length != ze.Length)
                        {
                            throw new IOException("Failed to read zip entry: " + ze.FullName);
                        }
                        _documentEntries[ze.FullName] = buf;
                    }
                }
            }
        }

        public virtual async Task SaveAsync(Stream outStream)
        {
            using (var zip = new ZipArchive(outStream, ZipArchiveMode.Create))
            {
                foreach (var item in _documentEntries)
                {
                    await this.AddZipEntryAsync(zip, item.Key);
                }
            }
        }

        public virtual void Save(Stream outStream) =>
            this.SaveAsync(outStream).GetAwaiter().GetResult();

        protected async Task AddZipEntryAsync(ZipArchive archive, string name)
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
            using (var zs = zae.Open())
            {
                await zs.WriteAsync(data, 0, data.Length);
            }
        }

        public IEnumerable<string> EntryPaths
        {
            get { return this._documentEntries.Keys; }
        }

        public byte[] GetEntryBuffer(string entryPath) => _documentEntries[entryPath];

        public void SetEntryBuffer(string entryPath, byte[] buffer) => _documentEntries[entryPath] = buffer;

        public bool EntryExists(string entryPath)
        {
            if (string.IsNullOrEmpty(entryPath))
            {
                throw new ArgumentNullException(nameof(entryPath));
            }
            return this._documentEntries.ContainsKey(entryPath);
        }

        public virtual byte[] AsBuffer()
        {
            using (var ms = new MemoryStream())
            {
                this.Save(ms);
                return ms.ToArray();
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

            var bufSize = 1024 * 8;
            var buf = new byte[bufSize];
            int nRead = 0;
            while ((nRead = src.Read(buf, 0, bufSize)) > 0)
            {
                dest.Write(buf, 0, nRead);
            }
        }

        public virtual void SaveAs(IZippedDocument destDoc)
        {
            if (destDoc == null)
            {
                throw new ArgumentNullException("destDoc");
            }

            //A Copy on write approach
            foreach (var item in this.EntryPaths)
            {
                using (var inStream = this.OpenEntryToRead(item))
                using (var outStream = destDoc.OpenOrCreateEntryToWrite(item))
                {
                    CopyStream(inStream, outStream);
                }
            }
        }
    }

}
