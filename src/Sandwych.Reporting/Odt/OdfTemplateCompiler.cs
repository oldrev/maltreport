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

            XNamespace text = mainDocument.NSManager.LookupNamespace("text");
            XNamespace xlink = mainDocument.NSManager.LookupNamespace("xlink");
            var xlinkHrefAttr = xlink + "href";

            var tlrLinks = mainDocument.NSAwaredXPathSelectElements("//text:a")
                .Where(e => e.Attribute(xlinkHrefAttr)?.Value?.StartsWith(WellknownConstants.TlrProtocolPrefix) ?? false)
                .ToArray();

            var tldLinks = mainDocument.NSAwaredXPathSelectElements("//text:a")
                .Where(e => e.Attribute(xlinkHrefAttr)?.Value?.StartsWith(WellknownConstants.TldProtocolPrefix) ?? false)
                .ToArray();

            foreach (var e in tlrLinks)
            {
                var href = e.Attribute(xlinkHrefAttr)?.Value;
                var expr = Utils.UrlUtility.UrlDecode(href.Substring(6), Encoding.UTF8).TrimEnd('/');

                e.ReplaceWith(new RawXText(expr));
            }

            foreach (var e in tldLinks)
            {
                var href = e.Attribute(xlinkHrefAttr)?.Value;
                var expr = Utils.UrlUtility.UrlDecode(href.Substring(6), Encoding.UTF8).TrimEnd('/');

                e.ReplaceWith(new DirectiveXElement(expr));
            }

            DirectiveXElement.SanitizeDirectiveElements(mainDocument);

            this.ProcessImageReferences(mainDocument);

            this.SanitizeReferenceElements(mainDocument);

            compiledDoc.WriteTextEntry(OdfDocument.ContentEntryPath, mainDocument.ToString());
            //  await compiledDoc.WriteXDocumentEntryAsync(DocxDocument.MainDocumentPath, mainDocument, ct);

            await compiledDoc.FlushAsync();

            return new OdtTemplate(compiledDoc);
        }

        private void ProcessImageReferences(NSAwaredXDocument mainDocument)
        {
            // Process Image tags
            // TODO FIXME
            XNamespace draw = mainDocument.NSManager.LookupNamespace("draw");
            XNamespace xlink = mainDocument.NSManager.LookupNamespace("xlink");
            var xlinkHrefAttr = xlink + "href";

            var drawFrames = mainDocument.NSAwaredXPathSelectElements("//draw:frame").ToArray();
            foreach (var drawFrame in drawFrames)
            {
                var drawImage = drawFrame.Descendants(draw + "image").SingleOrDefault();
                var drawNameValue = drawImage?.Attribute(draw + "name")?.Value?.Trim();
                if (drawImage != null
                    && !string.IsNullOrEmpty(drawNameValue)
                    && drawNameValue.StartsWith("{{")
                    && drawNameValue.EndsWith("}}"))
                {
                    // Repack the user expression
                    var userExpr = drawNameValue.Trim('{', '}');
                    var fluidExpr = "{{ " + userExpr + " | " + OdfImageFilter.FilterName + " }}";

                    drawImage.ReplaceWith(new RawXText(fluidExpr));
                    // TODO remove the placeholder's image
                }
            }
        }

        /// <summary>
        /// Sanitize template text
        /// </summary>
        private void SanitizeReferenceElements(NSAwaredXDocument doc)
        {
            /* Removes superfluous elements around the interpolation ( {﻿{...}} )
             e.g. <text:p text:style-name="P1">{{<text:span text:style-name="T2">so</text:span>.<text:span text:style-name="T2">StringValue</text:span>}}</text:p>
              is transformed in <text:p text:style-name="P1">{{so.StringValue}}</text:p>
            */

            // TODO: The following is very coarse grained, can probably be refined.
            var elements = doc.Descendants()
                .Where(x => x.Nodes().Any(x => (x as XText)?.Value?.Trim()?.StartsWith("{{") ?? false));
            foreach (var element in elements)
            {
                var expr = element.Value.Trim();
                element.RemoveAll(); // Remove all children
                element.Value = expr; // Set element's content
            }

        }

    }
}
