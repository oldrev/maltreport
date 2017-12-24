using System;
using System.IO;

namespace Sandwych.Reporting.IO
{
    /// <summary>
    ///  Memory stream of write-only
    /// </summary>
    internal sealed class ZipDocumentEntryOutputMemoryStream<TDocument> : MemoryStream
        where TDocument : AbstractZipDocument<TDocument>, new()
    {
        private readonly string _entryPath;
        private readonly AbstractZipDocument<TDocument> _zipDocument;

        public ZipDocumentEntryOutputMemoryStream(string name, AbstractZipDocument<TDocument> zipDocument)
        {
            this._entryPath = name;
            _zipDocument = zipDocument;
        }

        protected override void Dispose(bool disposing)
        {
            try
            {
                if (disposing)
                {
                    _zipDocument.SetEntryBuffer(this._entryPath, this.ToArray());
                }
            }
            finally
            {
                base.Dispose(disposing);
            }
        }

        public override bool CanRead
        {
            get
            {
                return false;
            }
        }

        public override bool CanSeek
        {
            get
            {
                return false;
            }
        }

        public override bool CanWrite
        {
            get
            {
                return true;
            }
        }

        public override int Read(byte[] buffer, int offset, int count)
        {
            throw new NotSupportedException();
        }

        public override int ReadByte()
        {
            throw new NotSupportedException();
        }

        public override long Position
        {
            get
            {
                return base.Position;
            }
            set
            {
                throw new NotSupportedException();
            }
        }

        public override long Seek(long offset, SeekOrigin loc)
        {
            throw new NotSupportedException();
        }
    }
}