using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Sandwych.Reporting.Textilize;

namespace Sandwych.Reporting
{
    public abstract class AbstractTemplate<TDocument>
        where TDocument : IDocument
    {
        private readonly TDocument _document;
        private readonly static IFluidFilter[] s_emptyFilters = new IFluidFilter[] { };

        public TDocument TemplateDocument => _document;

        public AbstractTemplate(TDocument document)
        {
            if (document.IsNew)
            {
                throw new ArgumentOutOfRangeException(nameof(document), "The template document must not be new(empty)");
            }

            _document = document;
            this.PrepareTemplate();
        }

        public virtual IDocument Render(TemplateContext context) =>
            Task.Run(() => this.RenderAsync(context)).Result;

        public abstract Task<IDocument> RenderAsync(TemplateContext context, CancellationToken ct = default);

        protected abstract void PrepareTemplate();

        protected static IFluidFilter[] EmptyFilters => s_emptyFilters;

        protected virtual IEnumerable<IFluidFilter> GetInternalFiltersToRegister(TDocument outputDocument) => s_emptyFilters;

        protected virtual FluidTemplateContext CreateFluidTemplateContext(TDocument outputDocument, TemplateContext context)
        {
            var ftc = new FluidTemplateContext(context.Values, context.Options);
            ftc.CultureInfo = context.CultureInfo;
            this.RegisterInternalFilters(outputDocument, ftc);
            return ftc;
        }

        private void RegisterInternalFilters(TDocument outputDocument, FluidTemplateContext templateContext)
        {
            foreach (var asyncFilter in this.GetInternalFiltersToRegister(outputDocument))
            {
                templateContext.Options.Filters.AddFilter(asyncFilter.Name, asyncFilter.InvokeAsync);
            }
        }

    }
}
