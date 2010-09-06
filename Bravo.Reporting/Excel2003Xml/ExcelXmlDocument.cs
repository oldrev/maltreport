//作者：李维
//创建时间：2010-09-03

using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;
using System.IO;
using System.Xml;

namespace Bravo.Reporting.Excel2003Xml
{
    public class ExcelXmlDocument : DocumentBase
    {
        public const string IndexAttribute = "ss:Index";
        public const string ExpandedColumnCountAttribute = "ss:ExpandedColumnCount";
        public const string ExpandedRowCountAttribute = "ss:ExpandedRowCount";
        public const string FormatAttribute = "ss:Format";
        public const string TypeAttribute = "ss:Type";

        private static readonly string[] entryPaths = new string[] { "xml" };
        private Dictionary<string, byte[]> entries = new Dictionary<string, byte[]>();

        public ExcelXmlDocument()
        {
        }

        public override void Load(Stream inStream)
        {
            this.entries.Clear();

            if (inStream.Length >= int.MaxValue)
            {
                throw new OverflowException();
            }

            var xmlSize = (int)inStream.Length;
            var buf = new byte[xmlSize];
            var nread = inStream.Read(buf, 0, xmlSize);
            if (nread != xmlSize)
            {
                throw new IOException();
            }

            this.entries["xml"] = buf;
        }

        public override void Save(Stream outStream)
        {
            using (var contentStream = this.GetEntryInputStream(this.MainContentEntryPath))
            {
                CopyStream(contentStream, outStream);
                outStream.Flush();
            }
        }

        public override Stream GetEntryInputStream(string entryPath)
        {
            if (string.IsNullOrEmpty(entryPath))
            {
                throw new ArgumentNullException("entryPath");
            }

            return new MemoryStream(this.entries[entryPath]);
        }

        public override Stream GetEntryOutputStream(string entryPath)
        {
            if (string.IsNullOrEmpty(entryPath))
            {
                throw new ArgumentNullException("entryPath");
            }

            return new OutputMemoryStream(entryPath, this.entries);
        }

        public override string MainContentEntryPath
        {
            get { return "xml"; }
        }

        public override ICollection<string> EntryPaths
        {
            get { return entryPaths; }
        }

        public override bool EntryExists(string entryPath)
        {
            return entryPath.ToLowerInvariant() == this.MainContentEntryPath;
        }

        public override string AddImage(Image img)
        {
            throw new NotSupportedException(
                "Microsoft Excel 2003 XML 文档不支持内嵌的图片");
        }

        public override ITemplate Compile()
        {
            var compiler = new ExcelXmlTemplateCompiler();
            return compiler.Compile(this);
        }
    }
}
