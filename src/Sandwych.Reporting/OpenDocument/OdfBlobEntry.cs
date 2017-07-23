namespace Sandwych.Reporting.OpenDocument
{
    public class OdfBlobEntry
    {
        public OdfBlobEntry(string entryPath, Blob blob)
        {
            this.EntryPath = entryPath;
            this.Blob = blob;
        }

        public string EntryPath { get; private set; }
        public Blob Blob { get; private set; }
    }
}