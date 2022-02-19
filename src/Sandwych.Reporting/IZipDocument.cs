using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace Sandwych.Reporting
{
    public interface IZipDocument : IDocument
    {

        byte[] GetEntryBuffer(string entryPath);
        void SetEntryBuffer(string entryPath, byte[] buffer);
        Task SetEntryAsync(string entryPath, Stream inStream);
        bool EntryExists(string entryPath);
        Stream OpenEntryToRead(string entryPath);
        Stream OpenOrCreateEntryToWrite(string entryPath);

    }
}
