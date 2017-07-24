namespace Sandwych.Reporting
{
    public class DocumentBlobEntry
    {
        public DocumentBlobEntry(string entryPath, Blob blob)
        {
            this.EntryPath = entryPath;
            this.Blob = blob;
        }

        public string EntryPath { get; private set; }
        public Blob Blob { get; private set; }
    }
}