using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;
using Sandwych.Reporting.Odf.Filters;
using Sandwych.Reporting.Xml;

namespace Sandwych.Reporting.Odf
{
    public class OdfTemplateCompiler : AbstractTemplateCompiler<OdfDocument>
    {
        public static readonly OdfTemplateCompiler Instance = new OdfTemplateCompiler();

        public override async Task<ITemplate> CompileAsync(OdfDocument doc, CancellationToken ct = default)
        {
            var compiledDoc = await doc.DuplicateAsync();
            var mainDocument = await compiledDoc.ReadXDocumentEntryAsync(OdfDocument.ContentEntryPath);

            using var relsDocumentStream = compiledDoc.OpenEntryToRead(compiledDoc.MainContentEntryPath);

            this.CompileHyperlinkStatements(mainDocument);

            this.CompileImageNameOutputStatements(mainDocument);

            this.CompileInputFieldAndPlaceholderStatements(mainDocument);

            var officeBodyElement = mainDocument.NSAwaredXPathSelectElement("/office:document-content/office:body");
            var officeBodyType = officeBodyElement.Elements().First().Name.LocalName;

            // Check is a .ODS file
            if (officeBodyType == "spreadsheet")
            {
                this.CompileOdsSpecified(mainDocument);
            }

            DirectiveXElement.SanitizeDirectiveElements(mainDocument);
            var mainDocumentText = mainDocument.ToString();

            compiledDoc.WriteTextEntry(OdfDocument.ContentEntryPath, mainDocumentText);

            await compiledDoc.FlushAsync();

            return officeBodyType switch
            {
                "spreadsheet" =>  new OdsTemplate(compiledDoc),
                "text" =>  new OdtTemplate(compiledDoc),
                _ => throw new NotSupportedException($"Not supported document type: {officeBodyType}"),
            };

        }

        private void CompileOdsSpecified(NSAwaredXDocument mainDocument)
        {
            var officeValueName = mainDocument.GetFullName("office", "value");
            var officeValueTypeName = mainDocument.GetFullName("office", "value-type");
            var calcextValueTypeName = mainDocument.GetFullName("calcext", "value-type");
            var tableStyleNameAttr = mainDocument.GetFullName("table", "style-name");

            var tableCellName = mainDocument.GetFullName("table", "table-cell");
            var tableCells = mainDocument.Root
                .Descendants(tableCellName)
                .Where(tc => tc.Value?.Trim()?.StartsWith("{{") ?? false)
                .ToArray();
            foreach (var cell in tableCells)
            {
                var rawUserExpr = cell.Value.Trim();
                var userExpr = rawUserExpr.Trim('{', '}').Trim();
                var tableStyleName = cell.Attribute(tableStyleNameAttr)?.Value ?? string.Empty;
                var filteredExpr = $" {{{{ {userExpr} | {OdfTypedTableCellFilter.FilterName}: '{tableStyleName}' }}}}\n";
                cell.ReplaceWith(new RawXText(filteredExpr));
            }
        }

        private void CompileInputFieldAndPlaceholderStatements(NSAwaredXDocument mainDocument)
        {
            XNamespace text = mainDocument.NSManager.LookupNamespace("text");
            var textDescriptionAttr = text + "description";

            var textPlaceholders = mainDocument.NSAwaredXPathSelectElements("//text:placeholder")
                .Where(e =>
                {
                    var expr = e.Attribute(textDescriptionAttr)?.Value.Trim();
                    return !string.IsNullOrEmpty(expr) && expr.StartsWith("{{") && expr.EndsWith("}}");
                }).ToArray();

            foreach (var textPlaceholder in textPlaceholders)
            {
                textPlaceholder.ReplaceWith(new RawXText(textPlaceholder.Attribute(textDescriptionAttr).Value));
            }

            var textInputs = mainDocument.NSAwaredXPathSelectElements("//text:text-input")
                .Where(e =>
                {
                    var expr = e.Attribute(textDescriptionAttr)?.Value.Trim();
                    return !string.IsNullOrEmpty(expr) && expr.StartsWith("{%") && expr.EndsWith("%}");
                }).ToArray();

            foreach (var textInput in textInputs)
            {
                var expr = textInput.Attribute(textDescriptionAttr).Value.Trim();
                textInput.ReplaceWith(new DirectiveXElement(expr));
            }
        }

        private void CompileHyperlinkStatements(NSAwaredXDocument mainDocument)
        {
            XNamespace text = mainDocument.NSManager.LookupNamespace("text");
            XNamespace xlink = mainDocument.NSManager.LookupNamespace("xlink");
            var xlinkHrefAttr = xlink + "href";

            var tloLinks = mainDocument.NSAwaredXPathSelectElements("//text:a")
                .Where(e => e.Attribute(xlinkHrefAttr)?.Value?.StartsWith(WellknownConstants.TloProtocolPrefix) ?? false)
                .ToArray();

            var tldLinks = mainDocument.NSAwaredXPathSelectElements("//text:a")
                .Where(e => e.Attribute(xlinkHrefAttr)?.Value?.StartsWith(WellknownConstants.TldProtocolPrefix) ?? false)
                .ToArray();

            foreach (var e in tloLinks)
            {
                var href = e.Attribute(xlinkHrefAttr)?.Value;
                var expr = Utils.UrlUtility.UrlDecode(href.Substring(6), Encoding.UTF8).TrimEnd('/');

                e.ReplaceWith(new RawXText("{{ " + expr + " }}"));
            }

            foreach (var e in tldLinks)
            {
                var href = e.Attribute(xlinkHrefAttr)?.Value;
                var expr = Utils.UrlUtility.UrlDecode(href.Substring(6), Encoding.UTF8).TrimEnd('/');

                e.ReplaceWith(new DirectiveXElement("{% " + expr + " %}"));
            }
        }

        private void CompileImageNameOutputStatements(NSAwaredXDocument mainDocument)
        {
            // Process Image tags
            // TODO FIXME
            XNamespace draw = mainDocument.NSManager.LookupNamespace("draw");
            XNamespace xlink = mainDocument.NSManager.LookupNamespace("xlink");
            XNamespace svg = mainDocument.NSManager.LookupNamespace("svg");
            var xlinkHrefAttr = xlink + "href";

            var drawFrames = mainDocument.NSAwaredXPathSelectElements("//draw:frame").ToArray();
            foreach (var drawFrame in drawFrames)
            {
                var svgDesc = drawFrame.Elements(svg + "desc").SingleOrDefault();
                var expr = svgDesc?.Value?.Trim();
                var drawImage = drawFrame.Descendants(draw + "image").SingleOrDefault();
                if (svgDesc != null && !string.IsNullOrEmpty(expr) && expr.StartsWith("{{") && expr.EndsWith("}}"))
                {
                    // Repack the user expression
                    var userExpr = expr.Trim('{', '}');
                    var fluidExpr = $"{{{{ {userExpr} | {OdfImageFilter.FilterName} }}}}";

                    if (drawImage != null)
                    {
                        drawImage.Remove();
                    }

                    svgDesc.ReplaceWith(new RawXText(fluidExpr));

                    // TODO remove the placeholder's image
                }
            }
        }

    }
}
