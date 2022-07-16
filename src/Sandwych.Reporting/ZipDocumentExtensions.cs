using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;
using Sandwych.Reporting.Xml;

namespace Sandwych.Reporting
{
    public static class ZipDocumentExtensions
    {

        public static TextReader GetEntryTextReader(this IZipDocument self, string entryPath) =>
            new StreamReader(self.OpenEntryToRead(entryPath),
                detectEncodingFromByteOrderMarks: false, encoding: Encoding.UTF8, bufferSize: 1024, leaveOpen: false);

        public static TextWriter GetEntryTextWriter(this IZipDocument self, string entryPath) =>
            new StreamWriter(self.OpenOrCreateEntryToWrite(entryPath),
                encoding: Encoding.UTF8, bufferSize: 1024, leaveOpen: false);

        public static void WriteXmlEntry(this IZipDocument self, string entryPath, XmlDocument xml)
        {
            using var cos = self.OpenOrCreateEntryToWrite(entryPath);
            using var writer = XmlWriter.Create(cos, new XmlWriterSettings { Encoding = Encoding.UTF8 });
            xml.WriteTo(writer);
            writer.Flush();
            cos.Flush();
        }

        public static XmlDocument ReadXmlEntry(this IZipDocument self, string entryPath)
        {
            using var contentStream = self.OpenEntryToRead(entryPath);
            var xml = new XmlDocument();
            xml.Load(contentStream);
            return xml;
        }

        public static string ReadTextEntry(this IZipDocument self, string entryPath)
        {
            using var tr = self.GetEntryTextReader(entryPath);
            return tr.ReadToEnd();
        }

        public static void WriteTextEntry(this IZipDocument self, string entryPath, string content)
        {
            using var tw = self.GetEntryTextWriter(entryPath);
            tw.Write(content);
            tw.Flush();
        }

        public static async Task<string> ReadTextEntryAsync(this IZipDocument self, string entryPath)
        {
            using var tr = self.GetEntryTextReader(entryPath);
            return await tr.ReadToEndAsync();
        }

        public static async Task WriteTextEntryAsync(this IZipDocument self,
            string entryPath, string content)
        {
            using var tw = self.GetEntryTextWriter(entryPath);
            await tw.WriteAsync(content);
            await tw.FlushAsync();
        }

        public static async Task<NSAwaredXDocument> ReadXDocumentEntryAsync(this IZipDocument self, 
            string entryPath, CancellationToken ct = default)
        {
            using var inStream = self.OpenEntryToRead(entryPath);
            return await NSAwaredXDocument.LoadFromAsync(inStream, ct);
        }

        public static async Task WriteXDocumentEntryAsync(this IZipDocument self, 
            string entryPath, XDocument xdoc, CancellationToken ct = default)
        {
            using var inStream = self.OpenOrCreateEntryToWrite(entryPath);
#if NET5_0_OR_GREATER || NETCOREAPP3_1_OR_GREATER || NETSTANDARD2_1_OR_GREATER
            await xdoc.SaveAsync(inStream, SaveOptions.None, ct);
#else
            await Task.Factory.StartNew(() => xdoc.Save(inStream, SaveOptions.None)).ConfigureAwait(false);
#endif
            await inStream.FlushAsync();
        }

    }
}
