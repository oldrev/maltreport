using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace Bravo.Reporting
{
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
    }
}
