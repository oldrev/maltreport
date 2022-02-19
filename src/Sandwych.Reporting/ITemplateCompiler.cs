using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Sandwych.Reporting
{
    public abstract class AbstractTemplateCompiler<TDocument>
        where TDocument : IDocument, new()
    {
        public abstract Task<ITemplate> CompileAsync(TDocument doc, CancellationToken ct = default);
    }
}
