using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Threading.Tasks;

namespace Sandwych.Reporting
{
    public interface IDocument 
    {
        Task LoadAsync(Stream inStream);

        void Load(Stream inStream);

        Task SaveAsync(Stream outStream);

        void Save(Stream outStream);
    }
}
