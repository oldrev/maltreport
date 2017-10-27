using Fluid;
using Sandwych.Reporting.Textilize;
using System.IO;
using System.Linq;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using Sandwych.Reporting.OpenDocument.Filters;
using System.Collections.Generic;

namespace Sandwych.Reporting.OpenDocument
{
    public abstract class OdfTemplate : AbstractTemplate<OdfDocument>
    {
        private FluidTemplate _fluidTemplate = null;

        public OdfTemplate(OdfDocument templateDocument) : base(templateDocument)
        {

        }

        public override async Task<OdfDocument> RenderAsync(TemplateContext context)
        {
            var outputDocument = new OdfDocument();
            this.TemplateDocument.SaveAs(outputDocument);

            var mainContentTemplate = this.TemplateDocument.ReadTextEntry(this.TemplateDocument.MainContentEntryPath);

            var fluidContext = this.CreateFluidTemplateContext(outputDocument, context);
            using (var ws = outputDocument.OpenOrCreateEntryToWrite(outputDocument.MainContentEntryPath))
            using (var writer = new StreamWriter(ws))
            {
                await _fluidTemplate.RenderAsync(writer, HtmlEncoder.Default, fluidContext);
            }

            outputDocument.Flush();
            return outputDocument;
        }

        public override OdfDocument Render(TemplateContext context) =>
            this.RenderAsync(context).GetAwaiter().GetResult();

        protected override IEnumerable<ISyncFilter> GetInternalSyncFilters(OdfDocument document)
        {
            yield return new OdfImageFilter(document);
        }

        protected override void CompileAndParse()
        {
            this.TemplateDocument.Compile();

            this.TemplateDocument.Flush();

            var mainContentText = this.TemplateDocument.GetEntryTextReader(this.TemplateDocument.MainContentEntryPath).ReadToEnd();
            if (!FluidTemplate.TryParse(mainContentText, out this._fluidTemplate, out var errors))
            {
                throw new SyntaxErrorException(errors.Aggregate((x, y) => x + "\n" + y));
            }
        }
    }
}