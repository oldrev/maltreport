using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Sandwych.Reporting
{
    public interface IDocument
    {
        bool IsNew { get; }
        Task SaveAsync(Stream outStream, CancellationToken ct = default);
        Task LoadAsync(Stream inStream, CancellationToken ct = default);
    }
}
