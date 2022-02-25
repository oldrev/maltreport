using System;

namespace Sandwych.Reporting
{
    public class DocumentBlobEntry : IEquatable<DocumentBlobEntry>
    {
        public DocumentBlobEntry(string entryPath, Blob blob)
        {
            this.EntryPath = entryPath;
            this.Blob = blob;
        }

        public string EntryPath { get; }
        public Blob Blob { get; }

        public override bool Equals(object other)
        {
            if (other == null)
            {
                return false;
            }

            if (ReferenceEquals(this, other))
            {
                return true;
            }

            if (other is DocumentBlobEntry dbe)
            {
                return this.Equals(dbe);
            }

            return false;
        }

        public bool Equals(DocumentBlobEntry other)
        {
            if (other == null)
            {
                return false;
            }

            if (ReferenceEquals(this, other))
            {
                return true;
            }

            return this.EntryPath.Equals(other.EntryPath) && this.Blob.Equals(other.Blob);
        }

        public override int GetHashCode() => this.Blob.GetHashCode();

    }
}