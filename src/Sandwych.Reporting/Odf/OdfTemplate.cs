using Fluid;
using Sandwych.Reporting.Textilize;
using System.IO;
using System.Linq;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using Sandwych.Reporting.Odf.Filters;
using System.Collections.Generic;

namespace Sandwych.Reporting.Odf
{
    using System.Threading;
    using System.Xml;
    using System.Xml.Linq;
    using Fluid.Parser;

    public abstract class OdfTemplate : AbstractTemplate<OdfDocument>
    {
        private IFluidTemplate _mainFluidTemplate = null;

        public OdfTemplate(OdfDocument templateDocument) : base(templateDocument)
        {

        }

        public override async Task<IDocument> RenderAsync(TemplateContext context, CancellationToken ct = default)
        {
            await this.LoadAndParseFluidTemplateAsync();

            var outputDocument = await this.CompiledTemplateDocument.DuplicateAsync(ct);

            var fluidContext = this.CreateFluidTemplateContext(outputDocument, context);
            using var ws = outputDocument.OpenOrCreateEntryToWrite(outputDocument.MainContentEntryPath);
            using var writer = new StreamWriter(ws);
            await _mainFluidTemplate.RenderAsync(writer, HtmlEncoder.Default, fluidContext);
            await writer.FlushAsync();
            await outputDocument.FlushAsync();
            return outputDocument;
        }

        protected override IEnumerable<IFluidFilter> GetInternalFiltersToRegister(OdfDocument document)
        {
            yield return new OdfImageFilter(document);
            yield return new OdfTypedValueFilter();
        }

        protected override void PrepareTemplate()
        {
            /*
            this.CompiledTemplateDocument.Compile();

            this.CompiledTemplateDocument.Flush();

            var mainContentText = this.CompiledTemplateDocument.GetEntryTextReader(this.CompiledTemplateDocument.MainContentEntryPath).ReadToEnd();
            var sanitizedMainContentText = this.SanitizeXml(mainContentText);

            var parser = FluidParserHolder.Instance;
            if (!parser.TryParse(sanitizedMainContentText, out _mainFluidTemplate, out var error))
            {
                throw new SyntaxErrorException(error);
            }
            */
        }

        private async Task LoadAndParseFluidTemplateAsync()
        {
            if (_mainFluidTemplate == null)
            {
                var reader = this.CompiledTemplateDocument.GetEntryTextReader(OdfDocument.ContentEntryPath);
                var mainDocumentText = await reader.ReadToEndAsync();
                _mainFluidTemplate = FluidParserHolder.Instance.Parse(mainDocumentText);
            }
        }


    }
}