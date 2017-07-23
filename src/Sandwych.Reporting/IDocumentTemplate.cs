using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Threading.Tasks;

namespace Sandwych.Reporting
{
    public interface IDocumentTemplate
    {

        Task<IDocument> RenderAsync(TemplateContext context);

        IDocument Render(TemplateContext context);
    }
}
