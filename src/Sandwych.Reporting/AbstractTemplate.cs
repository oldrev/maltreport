using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Sandwych.Reporting.Textilize;

namespace Sandwych.Reporting
{
    public abstract class AbstractTemplate<TDocument> : ITemplate<TDocument>
        where TDocument : IDocument, new()
    {
        private readonly TDocument _document;
        private readonly static ISyncFilter[] s_emptySyncFilters = new ISyncFilter[] { };
        private readonly static IAsyncFilter[] s_emptyAsyncFilters = new IAsyncFilter[] { };

        public TDocument TemplateDocument => _document;

        public AbstractTemplate(TDocument document)
        {
            _document = document;
            this.CompileAndParse();
        }

        public abstract TDocument Render(TemplateContext context);

        public abstract Task<TDocument> RenderAsync(TemplateContext context);

        protected abstract void CompileAndParse();

        protected static ISyncFilter[] EmptySyncFilters => s_emptySyncFilters;

        protected static IAsyncFilter[] EmptyAsyncFilters => s_emptyAsyncFilters;

        protected virtual IEnumerable<ISyncFilter> GetInternalSyncFilters(TDocument document) => s_emptySyncFilters;

        protected virtual IEnumerable<IAsyncFilter> GetInternalAsyncFilters(TDocument document) => s_emptyAsyncFilters;

        protected virtual FluidTemplateContext CreateFluidTemplateContext(TDocument document, TemplateContext context)
        {
            var ftc = new FluidTemplateContext(context.Values);
            this.RegisterInternalFilters(document, ftc);
            return ftc;
        }

        private void RegisterInternalFilters(TDocument document, FluidTemplateContext templateContext)
        {
            foreach (var syncFilter in this.GetInternalSyncFilters(document))
            {
                templateContext.Filters.AddFilter(syncFilter.Name, syncFilter.Execute);
            }

            foreach (var asyncFilter in this.GetInternalAsyncFilters(document))
            {
                templateContext.Filters.AddAsyncFilter(asyncFilter.Name, asyncFilter.ExecuteAsync);
            }
        }

    }
}
