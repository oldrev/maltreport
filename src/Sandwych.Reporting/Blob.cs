using System;
using System.Security.Cryptography;

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
    /// Blob in zipped document
    /// </summary>
    public abstract class Blob : IBlob
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
            using (var md5 = MD5.Create())
            {
                var hash = md5.ComputeHash(this._blobBuffer);
                return BitConverter.ToString(hash).Replace("-", "").ToLowerInvariant();
            }
        }

        private string MakeDocumentFileName() =>
            $"__blob_{this.Id}.{this.ExtensionName}";

        public string Id { get; }

        public string ExtensionName { get; }

        public byte[] GetBuffer() => _blobBuffer;

        public int Length => _blobBuffer.Length;

        public string FileName => _entryNameInDocument;

        public bool Equals(IBlob obj)
        {
            if (ReferenceEquals(this, obj))
            {
                return true;
            }

            if (obj == null)
            {
                return false;
            }

            if (this.Id == obj.Id)
            {
                return true;
            }

            return false;
        }

        public override int GetHashCode() => this.Id.GetHashCode();
    }
}