using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace Sandwych.Reporting
{
    public abstract class AbstractTemplate<TDocument> : ITemplate<TDocument>
        where TDocument : IDocument, new()
    {
        private readonly TDocument _document;

        public TDocument TemplateDocument => _document;

        public AbstractTemplate(TDocument document)
        {
            _document = document;
            this.CompileAndParse();
        }

        public abstract TDocument Render(TemplateContext context);

        public abstract Task<TDocument> RenderAsync(TemplateContext context);

        protected abstract void CompileAndParse();
    }
}
