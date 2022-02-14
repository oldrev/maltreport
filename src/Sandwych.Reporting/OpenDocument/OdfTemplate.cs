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
    using System.Xml;
    using System.Xml.Linq;
    using Fluid.Parser;

    public abstract class OdfTemplate : AbstractTemplate<OdfDocument>
    {
        private IFluidTemplate _fluidTemplate = null;

        public OdfTemplate(OdfDocument templateDocument) : base(templateDocument)
        {

        }

        public override async Task<OdfDocument> RenderAsync(TemplateContext context)
        {
            var outputDocument = new OdfDocument();
            await this.TemplateDocument.SaveAsAsync(outputDocument);

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

        protected override IEnumerable<IAsyncFilter> GetInternalAsyncFilters(OdfDocument document)
        {
            yield return new OdfImageFilter(document);
        }

        protected override void PrepareTemplate()
        {
            this.TemplateDocument.Compile();

            this.TemplateDocument.Flush();

            var mainContentText = this.TemplateDocument.GetEntryTextReader(this.TemplateDocument.MainContentEntryPath).ReadToEnd();
            var sanitizedMainContentText = Sanitize(mainContentText);

            var parser = new FluidParser();
            if (!parser.TryParse(sanitizedMainContentText, out this._fluidTemplate, out var errors))
            {
                throw new SyntaxErrorException(errors);
            }
        }

        /// <summary>
        /// Removes superfluous elements around the interpolation ( {﻿{...}} )
        ///
        /// e.g. <text:p text:style-name="P1">{{<text:span text:style-name="T2">so</text:span>.<text:span text:style-name="T2">StringValue</text:span>}}</text:p>
        ///      is transformed in
        ///      <text:p text:style-name="P1">{{so.StringValue}}</text:p>
        /// </summary>
        /// <param name="mainContentText"></param>
        /// <returns>Sanitized text</returns>
        private static string Sanitize(string mainContentText)
        {
            var doc = XDocument.Parse(mainContentText);

            // TODO: Is very coarse grained, can probably be refined.
            foreach (var element in doc.Descendants().Where(
                x => x.Nodes().Any(y => y.NodeType == XmlNodeType.Text && ((XText)y).Value.Contains("{{"))))
            {
                element.Value = element.Value;
            }

            return doc.ToString();
        }
    }
}