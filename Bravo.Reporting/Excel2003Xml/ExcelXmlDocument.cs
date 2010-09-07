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
    public class ExcelXmlDocument : IDocument
    {
        public const string IndexAttribute = "ss:Index";
        public const string ExpandedColumnCountAttribute = "ss:ExpandedColumnCount";
        public const string ExpandedRowCountAttribute = "ss:ExpandedRowCount";
        public const string FormatAttribute = "ss:Format";
        public const string TypeAttribute = "ss:Type";

        private byte[] data;

        public ExcelXmlDocument()
        {
        }

        public void Load(Stream inStream)
        {
            if (inStream == null)
            {
                throw new ArgumentNullException("inStream");
            }

            var size = (int)inStream.Length;
            this.data = new byte[size];
            var nread = inStream.Read(data, 0, size);

            if (nread != size)
            {
                throw new IOException();
            }
        }

        public void Save(Stream outStream)
        {
            Debug.Assert(this.data != null);

            if (outStream == null)
            {
                throw new ArgumentNullException("outStream");
            }

            outStream.Write(this.data, 0, this.data.Length);
        }

        public virtual ITemplate Compile()
        {
            return ExcelXmlCompiler.Compile(this);
        }

        #region IDocument 成员


        public void Save(string path)
        {
            using (var fs = File.Open(path, FileMode.Create, FileAccess.Write))
            {
                this.Save(fs);
            }
        }

        public void Load(string path)
        {
            using(var fs = File.OpenRead(path))
            {
                this.Load(fs);
            }
        }

        public byte[] GetBuffer()
        {
            return this.data;
        }

        #endregion

        #region ICloneable 成员

        public object Clone()
        {
            var o = new ExcelXmlDocument();
            o.PutBuffer(this.data);
            return o;
        }

        #endregion

        internal XmlDocument GetXmlDocument()
        {
            Debug.Assert(this.data != null);

            var xmldoc = new XmlDocument();
            using (var ms = new MemoryStream(this.data, false))
            {
                xmldoc.Load(ms);
            }
            return xmldoc;
        }

        internal MemoryStream GetInputStream()
        {
            return new MemoryStream(this.data, false);
        }

        internal void PutBuffer(byte[] buffer)
        {
            Debug.Assert(buffer != null);

            this.data = buffer;
        }
    }
}
