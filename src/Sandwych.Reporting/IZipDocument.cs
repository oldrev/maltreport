using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml;
using Sandwych.Reporting.IO;

namespace Sandwych.Reporting
{
    public interface IZipDocument : IDocument
    {
        IEnumerable<string> EntryPaths { get; }

        byte[] GetEntryBuffer(string entryPath);

        void SetEntryBuffer(string entryPath, byte[] buffer);

        bool EntryExists(string entryPath);

        void SaveAs(IZipDocument destDoc);
    }

    public static class ZippedDocumentExtensions
    {
        public static Stream OpenEntryToRead(this IZipDocument self, string entryPath)
        {
            return new MemoryStream(self.GetEntryBuffer(entryPath));
        }

        public static Stream OpenOrCreateEntryToWrite(this IZipDocument self, string entryPath)
        {
            var oms = new OutputMemoryStream(entryPath, self);
            return oms;
        }

        public static TextReader GetEntryTextReader(this IZipDocument self, string entryPath) =>
            new StreamReader(self.OpenEntryToRead(entryPath));

        public static TextWriter GetEntryTextWriter(this IZipDocument self, string entryPath) =>
            new StreamWriter(self.OpenOrCreateEntryToWrite(entryPath));

        public static void WriteXmlEntry(this IZipDocument self, string entryPath, XmlDocument xml)
        {
            using (var cos = self.OpenOrCreateEntryToWrite(entryPath))
            using (var writer = XmlWriter.Create(cos, new XmlWriterSettings { Encoding = Encoding.UTF8 }))
            {
                xml.WriteTo(writer);
            }
        }

        public static XmlDocument ReadXmlEntry(this IZipDocument self, string entryPath)
        {
            using (var contentStream = self.OpenEntryToRead(entryPath))
            {
                var xml = new XmlDocument();
                xml.Load(contentStream);
                return xml;
            }
        }

        public static string ReadTextEntry(this IZipDocument self, string entryPath)
        {
            using (var tr = self.GetEntryTextReader(entryPath))
            {
                return tr.ReadToEnd();
            }
        }

        public static void WriteTextEntry(this IZipDocument self, string entryPath, string content)
        {
            using (var tw = self.GetEntryTextWriter(entryPath))
            {
                tw.Write(content);
            }
        }
    }
}