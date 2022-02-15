namespace Sandwych.Reporting
{
    public class DocumentBlobEntry
    {
        public DocumentBlobEntry(string entryPath, Blob blob)
        {
            this.EntryPath = entryPath;
            this.Blob = blob;
        }

        public string EntryPath { get; }
        public Blob Blob { get; }
    }
}