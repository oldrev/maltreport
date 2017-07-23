using Fluid;
using Sandwych.Reporting.OpenDocument.Filters;
using Sandwych.Reporting.Textilize;
using System.IO;
using System.Linq;
using System.Text.Encodings.Web;
using System.Threading.Tasks;

namespace Sandwych.Reporting.OpenDocument
{
    public abstract class OdfTemplate : ITemplate<OdfDocument>
    {
        private readonly OdfDocument _document;
        private IFluidTemplate _fluidTemplate = null;

        public OdfTemplate(Stream inStream)
        {
            _document = new OdfDocument();
            _document.Load(inStream);
            this.CompileAndParse();
        }

        public OdfTemplate(string filePath)
        {
            _document = new OdfDocument();
            _document.Load(filePath);
            this.CompileAndParse();
        }

        public OdfTemplate(OdfDocument document)
        {
            _document = document;
            this.CompileAndParse();
        }

        public async Task<OdfDocument> RenderAsync(TemplateContext context)
        {
            var outputDocument = new OdfDocument();
            this._document.SaveAs(outputDocument);

            var mainContentTemplate = _document.ReadTextEntry(_document.MainContentEntryPath);

            this.SetInternalFilters(outputDocument, context.FluidContext);

            using (var ws = outputDocument.OpenOrCreateEntryToWrite(outputDocument.MainContentEntryPath))
            using (var writer = new StreamWriter(ws))
            {
                await _fluidTemplate.RenderAsync(writer, HtmlEncoder.Default, context.FluidContext);
            }

            outputDocument.Flush();
            return outputDocument;
        }

        protected virtual void SetInternalFilters(OdfDocument outputDocument, FluidTemplateContext templateContext)
        {
            var imageFilter = new OdfImageFilter(outputDocument);
            templateContext.Filters.AddFilter(imageFilter.Name, imageFilter.Execute);
        }

        public OdfDocument Render(TemplateContext context) =>
            this.RenderAsync(context).GetAwaiter().GetResult();

        private void CompileAndParse()
        {
            this._document.Compile();

            _document.Flush();

            var mainContentText = _document.GetEntryTextReader(_document.MainContentEntryPath).ReadToEnd();
            if (!FluidTemplate.TryParse(mainContentText, out this._fluidTemplate, out var errors))
            {
                throw new SyntaxErrorException(errors.Aggregate((x, y) => x + "\n" + y));
            }
        }
    }
}