using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Encodings.Web;
using System.Reflection;
using Fluid;
using Sandwych.Reporting.Textilize;
using System.Threading.Tasks;

namespace Sandwych.Reporting.OpenDocument
{
    public class OdfTemplate : ITemplate
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

        public async Task<IDocument> RenderAsync(IReadOnlyDictionary<string, object> context)
        {
            var outputDocument = new OdfDocument();
            this._document.SaveAs(outputDocument);

            var mainContentTemplate = _document.ReadTextEntry(_document.MainContentEntryPath);

            var templateContext = new FluidTemplateContext(outputDocument, context);

            var imageFilter = new OdfImageFilter(outputDocument);
            templateContext.Filters.AddFilter("image", imageFilter.Execute);

            using (var ws = outputDocument.OpenOrCreateEntryToWrite(outputDocument.MainContentEntryPath))
            using (var writer = new StreamWriter(ws))
            {
                await _fluidTemplate.RenderAsync(writer, HtmlEncoder.Default, templateContext);
            }

            outputDocument.Flush();
            return outputDocument;
        }

        public IDocument Render(IReadOnlyDictionary<string, object> context) =>
            this.RenderAsync(context).GetAwaiter().GetResult();

        private void CompileAndParse()
        {
            OdfCompiler.Compile(_document);
            _document.Flush();

            var mainContentText = _document.GetEntryTextReader(_document.MainContentEntryPath).ReadToEnd();
            if (!FluidTemplate.TryParse(mainContentText, out this._fluidTemplate, out var errors))
            {
                throw new SyntaxErrorException(errors.Aggregate((x, y) => x + "\n" + y));
            }
        }

    }
}
