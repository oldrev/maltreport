using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using Fluid;
using System.Threading.Tasks;

namespace Sandwych.Reporting
{
    public interface ITemplate
    {

        Task<IDocument> RenderAsync(IReadOnlyDictionary<string, object> context);

        IDocument Render(IReadOnlyDictionary<string, object> context);
    }
}
