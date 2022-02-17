using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Sandwych.Reporting
{
    public interface ITemplate
    {
        Task<IDocument> RenderAsync(TemplateContext context, CancellationToken ct = default);
    }
}
