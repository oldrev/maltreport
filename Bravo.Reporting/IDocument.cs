using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace Bravo.Reporting
{
    public interface IDocument
    {
        void Save(Stream outStream);
        void Save(string path);

        Stream GetEntryInputStream(string entryPath);
        Stream GetEntryOutputStream(string entryPath);

        string MainContentEntryPath { get; }

        byte[] GetBuffer();

        ICollection<string> EntryPaths { get; }

        bool EntryExists(string entryPath);

        string AddImage(Image img);

        void CopyTo(IDocument destDoc);
    }
}
