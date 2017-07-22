using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.IO;
using System.IO.Compression;
using System.Text;
using System.Xml;

namespace Sandwych.Reporting
{
    public interface IZippedDocument : IDocument
    {
        IEnumerable<string> EntryPaths { get; }
        Stream GetEntryInputStream(string entryPath);
        Stream GetEntryOutputStream(string entryPath);
        bool EntryExists(string entryPath);
        void SaveAs(IZippedDocument destDoc);
    }


    public static class ZippedDocumentExtensions
    {

        public static TextReader GetEntryTextReader(this IZippedDocument self, string entryPath) =>
            new StreamReader(self.GetEntryInputStream(entryPath));

        public static TextWriter GetEntryTextWriter(this IZippedDocument self, string entryPath) =>
            new StreamWriter(self.GetEntryOutputStream(entryPath));

        public static void WriteXmlEntry(this IZippedDocument self, string entryPath, XmlDocument xml)
        {
            using (var cos = self.GetEntryOutputStream(entryPath))
            using (var writer = XmlWriter.Create(cos, new XmlWriterSettings { Encoding = Encoding.UTF8 }))
            {
                xml.WriteTo(writer);
            }
        }

        public static XmlDocument ReadXmlEntry(this IZippedDocument self, string entryPath)
        {
            using (var contentStream = self.GetEntryInputStream(entryPath))
            {
                var xml = new XmlDocument();
                xml.Load(contentStream);
                return xml;
            }
        }

        public static string ReadTextEntry(this IZippedDocument self, string entryPath)
        {
            using (var tr = self.GetEntryTextReader(entryPath))
            {
                return tr.ReadToEnd();
            }
        }

        public static void WriteTextEntry(this IZippedDocument self, string entryPath, string content)
        {
            using (var tw = self.GetEntryTextWriter(entryPath))
            {
                tw.Write(content);
            }

        }
    }
}
