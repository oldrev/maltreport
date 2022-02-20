using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Sandwych.Reporting.Odf
{
    public class OdtTemplateCompiler : AbstractTemplateCompiler<OdfDocument>
    {
        public static readonly OdtTemplateCompiler Instance = new OdtTemplateCompiler();

        public override async Task<ITemplate> CompileAsync(OdfDocument doc, CancellationToken ct = default)
        {
            var compiledDoc = await doc.DuplicateAsync();
            var mainDocument = await compiledDoc.ReadXDocumentEntryAsync(OdfDocument.ContentEntryPath);

            using var relsDocumentStream = compiledDoc.OpenEntryToRead(compiledDoc.MainContentEntryPath);
            XNamespace r = mainDocument.NSManager.LookupNamespace("r");
            XNamespace w = mainDocument.NSManager.LookupNamespace("w");

            /*
            var tlrLinks = mainDocument.NSAwaredXPathSelectElements("//w:hyperlink")
                .Where(e => e.Attribute(r + "id")?.Value?.StartsWith("rId") ?? false)
                .Where(e => compiledDoc.MainRels.Entries.TryGetValue(e.Attribute(r + "id")?.Value, out var re)
                            && re.Target.StartsWith(WellknownConstants.TlrProtocolPrefix))
                .ToArray();

            var tldLinks = mainDocument.NSAwaredXPathSelectElements("//w:hyperlink")
                .Where(e => e.Attribute(r + "id")?.Value?.StartsWith("rId") ?? false)
                .Where(e => compiledDoc.MainRels.Entries.TryGetValue(e.Attribute(r + "id")?.Value, out var re)
                            && re.Target.StartsWith(WellknownConstants.TldProtocolPrefix))
                .ToArray();

            foreach (var e in tlrLinks)
            {
                var rid = e.Attribute(r + "id")?.Value;
                var linkRelEntry = compiledDoc.MainRels.Entries[rid];
                var expr = Utils.UrlUtility.UrlDecode(linkRelEntry.Target.Substring(6), Encoding.UTF8)
                    .TrimEnd('/');

                e.ReplaceWith(new ExpressionRunTextXElement(expr, w));
                compiledDoc.MainRels.RemoveEntry(rid);
            }

            foreach (var e in tldLinks)
            {
                var rid = e.Attribute(r + "id")?.Value;
                var linkRelEntry = compiledDoc.MainRels.Entries[rid];
                var expr = Utils.UrlUtility.UrlDecode(linkRelEntry.Target.Substring(6), Encoding.UTF8)
                    .TrimEnd('/');

                e.ReplaceWith(new DirectiveXElement(expr));
                compiledDoc.MainRels.RemoveEntry(rid);
            }

            DirectiveXElement.ReduceDirectiveElements(mainDocument);

            this.ProcessImageReferences(mainDocument);

            compiledDoc.WriteTextEntry(DocxDocument.MainDocumentPath, mainDocument.ToString());
            //  await compiledDoc.WriteXDocumentEntryAsync(DocxDocument.MainDocumentPath, mainDocument, ct);

            await compiledDoc.FlushAsync();

            */
            return new OdtTemplate(compiledDoc);
        }


    }
}
