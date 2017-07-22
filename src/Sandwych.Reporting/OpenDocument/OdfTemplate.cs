using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Encodings.Web;
using System.Reflection;
using Fluid;
using Sandwych.Reporting.Textilize;

namespace Sandwych.Reporting.OpenDocument
{
    public class OdfTemplate : ITemplate
    {
        private readonly OdfDocument _document;
        private IFluidTemplate _fluidTemplate = null;

        public OdfTemplate(OdfDocument document)
        {
            _document = document;
            this.CompileAndParse();
        }

        public IDocument Render(IReadOnlyDictionary<string, object> context)
        {
            var outputDocument = new OdfDocument();
            _document.SaveAs(outputDocument);

            var mainContentTemplate = _document.ReadTextEntry(_document.MainContentEntryPath);

            var templateContext = new FluidTemplateContext(outputDocument, context);

            var imageFilter = new OdfImageFilter(outputDocument);
            templateContext.Filters.AddFilter("image", imageFilter.Execute);

            using (var ws = outputDocument.GetEntryOutputStream(outputDocument.MainContentEntryPath))
            using (var writer = new StreamWriter(ws))
            {
                _fluidTemplate.Render(writer, HtmlEncoder.Default, templateContext);
            }

            return outputDocument;
        }

        private void CompileAndParse()
        {
            OdfCompiler.Compile(_document);
            var mainContentText = _document.GetEntryTextReader(_document.MainContentEntryPath).ReadToEnd();
            if (!FluidTemplate.TryParse(mainContentText, out this._fluidTemplate, out var errors))
            {
                throw new SyntaxErrorException(errors.Aggregate((x, y) => x + "\n" + y));
            }
        }

    }
}
