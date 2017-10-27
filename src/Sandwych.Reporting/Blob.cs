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
        private readonly string _extName;
        private readonly byte[] blob;
        private readonly string _entryNameInDocument;
        private readonly string _blobHash;

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

            this._extName = extensionName.ToLowerInvariant();
            this.blob = buffer;
            this._blobHash = ComputeBlobMD5Hash();
            this._entryNameInDocument = this.MakeDocumentFileName();
        }

        private string ComputeBlobMD5Hash()
        {
            using (var md5 = MD5.Create())
            {
                var hash = md5.ComputeHash(this.blob);
                return BitConverter.ToString(hash).Replace("-", "").ToLowerInvariant();
            }
        }

        private string MakeDocumentFileName() =>
            $"__blob_{_blobHash}.{this.ExtensionName}";

        public string Id => _blobHash;

        public string ExtensionName => _extName;

        public byte[] GetBuffer() => blob;

        public int Length => blob.Length;

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

        public override int GetHashCode() => _blobHash.GetHashCode();
    }
}