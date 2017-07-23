//作者：李维
//创建时间：2010-09-09
using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;
using System.IO;
using System.Xml;
using System.Threading.Tasks;

namespace Sandwych.Reporting
{
    public abstract class AbstractXmlTemplate<TDocument> : ITemplate<TDocument>
        where TDocument : IDocument
    {
        public abstract Task<TDocument> RenderAsync(TemplateContext context);

        public abstract TDocument Render(TemplateContext context);
    }
}
