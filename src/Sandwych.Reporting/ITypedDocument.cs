using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Sandwych.Reporting
{
    public interface ITypedDocument<TDocument> : IDocument where TDocument : IDocument, ITypedDocument<TDocument>
    {
        Task<TDocument> DuplicateAsync(CancellationToken ct = default);
    }
}
