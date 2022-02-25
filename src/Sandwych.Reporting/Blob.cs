using System;
using System.IO;
using System.Security.Cryptography;
using System.Threading;
using System.Threading.Tasks;

namespace Sandwych.Reporting
{

    public interface IBlob : IEquatable<IBlob>
    {
        string Id { get; }
        string ExtensionName { get; }
        byte[] GetBuffer();
        int Length { get; }
        string FileName { get; }
    }

    /// <summary>
    /// Blob in document
    /// </summary>
    public class Blob : IBlob
    {
        private readonly byte[] _blobBuffer;
        private readonly string _entryNameInDocument;

        public Blob(string extensionName, byte[] buffer)
        {
            if (string.IsNullOrEmpty(extensionName))
            {
                throw new ArgumentNullException(nameof(extensionName));
            }

            if (extensionName.StartsWith("."))
            {
                throw new ArgumentOutOfRangeException("Bad extension name, it should like 'png' or 'gif' etc.");
            }

            if (buffer == null || buffer.Length <= 0)
            {
                throw new ArgumentNullException(nameof(buffer));
            }

            this.ExtensionName = extensionName.ToLowerInvariant();
            _blobBuffer = buffer;
            this.Id = ComputeBlobMD5Hash();
            _entryNameInDocument = this.MakeDocumentFileName();
        }

        private string ComputeBlobMD5Hash()
        {
            using var md5 = MD5.Create();
            var hash = md5.ComputeHash(this._blobBuffer);
            return BitConverter.ToString(hash).Replace("-", "").ToLowerInvariant();
        }

        private string MakeDocumentFileName() =>
            $"__blob_{this.Id}.{this.ExtensionName}";

        public string Id { get; }

        public string ExtensionName { get; }

        public byte[] GetBuffer() => _blobBuffer;

        public int Length => _blobBuffer.Length;

        public string FileName => _entryNameInDocument;

        public override bool Equals(object obj)
        {
            if(obj == null)
            {
                return false;
            }

            if(ReferenceEquals(this, obj))
            {
                return true;
            }

            if(obj is IBlob blob)
            {
                return this.Equals(blob);
            }

            return false;
        }

        public bool Equals(IBlob obj)
        {
            if (obj == null)
            {
                return false;
            }

            if (ReferenceEquals(this, obj))
            {
                return true;
            }

            if (this.Id == obj.Id)
            {
                return true;
            }

            return false;
        }

        public override int GetHashCode() => this.Id.GetHashCode();

        public async static Task<Blob> LoadAsync(string filePath, CancellationToken cancellationToken = default)
        {
            using var fs = File.OpenRead(filePath);
            var buf = new byte[fs.Length];
            var nread = await fs.ReadAsync(buf, 0, (int)fs.Length, cancellationToken);
            if (nread != (int)fs.Length)
            {
                throw new IOException();
            }
            return new Blob(Path.GetExtension(filePath).TrimStart('.'), buf);
        }

        public async static Task<Blob> LoadAsync(Stream inStream, string extensionName, CancellationToken cancellationToken = default)
        {
            using var ms = new MemoryStream();
#if NETSTANDARD && NETSTANDARD2_0
            await inStream.CopyToAsync(ms);
#else
            await inStream.CopyToAsync(ms, cancellationToken);
#endif
            return new Blob(extensionName, ms.ToArray());
        }
    }
}