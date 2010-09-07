using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace Bravo.Reporting
{
    public interface IDocument : ICloneable
    {
        void Save(Stream outStream);
        void Save(string path);

        void Load(Stream inStream);
        void Load(string path);

        byte[] GetBuffer();

        ITemplate Compile();
    }
}
