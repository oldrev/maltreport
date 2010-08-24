//作者：李维
//创建时间：2010-08-20

using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace Bravo.Reporting
{
    /// <summary>
    ///  只写的内存字节流
    /// </summary>
    internal class OutputMemoryStream : MemoryStream
    {
        private string name;
        private IDictionary<string, byte[]> odfEntries;

        internal OutputMemoryStream(string name, IDictionary<string, byte[]> odfEntries)
        {
            this.name = name;
            this.odfEntries = odfEntries;
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);

            this.odfEntries[this.name] = this.ToArray();
        }

        public override bool CanRead
        {
            get
            {
                return false;
            }
        }

        public override int Read(byte[] buffer, int offset, int count)
        {
            throw new NotSupportedException();
        }

        public override int ReadByte()
        {
            throw new NotSupportedException();
        }
    }
}
