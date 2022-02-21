using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;
using System.IO;
using System.Xml;
using System.Threading.Tasks;
using Fluid;
using Sandwych.Reporting.Xml;
using System.Linq;
using System.Text.RegularExpressions;
using System.Text.Encodings.Web;
using Sandwych.Reporting.OfficeML.Filters;
using Sandwych.Reporting.Textilize;
using System.Threading;
using Sandwych.Reporting.Utils;

namespace Sandwych.Reporting.OfficeML
{
    public class WordMLTemplate : AbstractXmlTemplate<WordMLDocument>
    {
        private static readonly Lazy<Regex> ImageFormatPattern =
            new Lazy<Regex>(() => new Regex(@"^.*\|\s*image\s*:\s*'(.*)'\s*$", RegexOptions.Compiled | RegexOptions.CultureInvariant | RegexOptions.Singleline), true);

        public WordMLTemplate(WordMLDocument templateDocument) : base(templateDocument)
        {
        }

        protected override void PrepareTemplate()
        {
            this.ProcessPlaceholders();
        }

        public override async Task<IDocument> RenderAsync(TemplateContext context, CancellationToken ct = default)
        {
            var fluidContext = this.CreateFluidTemplateContext(null, context);
            using var outputWriter = new StringWriter();
            await this.TextTemplate.RenderAsync(outputWriter, HtmlEncoder.Default, fluidContext);
            return WordMLDocument.LoadFromText(outputWriter.ToString());
        }

        protected override IEnumerable<IFluidFilter> GetInternalFiltersToRegister(WordMLDocument document)
        {
            yield return new WordMLImageFilter();
        }

        private void ProcessPlaceholders()
        {
            var placeholders = this.FindAllPlaceholders();

            foreach (XmlElement phe in placeholders)
            {
                var attr = phe.GetAttribute(WordMLDocument.DestAttribute);
                var value = UrlUtility.UrlDecode(attr, Encoding.UTF8);
                value = value.Substring(WellknownConstants.TlrProtocolPrefix.Length).Trim('/').Trim();

                if (value.Length < 2)
                {
                    throw new SyntaxErrorException();
                }

                if (value.Trim().StartsWith("@"))
                {
                    this.ProcessDirectiveTag(phe, value.Substring(1));
                }
                else if (value.Trim().StartsWith("$"))
                {
                    this.ProcessReferenceTag(phe, value.Substring(1));
                }
                else
                {
                    throw new SyntaxErrorException($"Syntax error: '{attr}'");
                }
            }

            var imagePlaceholderElements = this.MatchImagePlaceholderElements();
            foreach (XmlElement ele in imagePlaceholderElements)
            {
                this.ProcessImagePlaceholderElement(ele);
            }
        }

        private void ProcessDirectiveTag(XmlElement placeholderNode, string value)
        {
            var directive = new DirectiveElement(
                this.CompiledTemplateDocument.XmlDocument, value);
            directive.ReduceTagByDirective(placeholderNode);
        }

        private List<XmlElement> FindAllPlaceholders()
        {
            var placeholders = new List<XmlElement>();
            var allNodes = this.CompiledTemplateDocument
                .XmlDocument.GetElementsByTagName(WordMLDocument.HlinkElement);

            foreach (XmlElement e in allNodes)
            {
                var attr = e.GetAttribute(WordMLDocument.DestAttribute);
                if (attr.StartsWith(WellknownConstants.TlrProtocolPrefix))
                {
                    placeholders.Add(e);
                }
            }

            return placeholders;
        }

        private void ProcessReferenceTag(XmlElement placeholderElement, string value)
        {
            var xml = this.CompiledTemplateDocument.XmlDocument;
            var refEle = new OutputXmlElement(xml, value);
            var rEle = xml.CreateElement("w:r", WordMLNamespaceManager.WNamespace);
            var tEle = xml.CreateElement("w:t", WordMLNamespaceManager.WNamespace);
            rEle.AppendChild(tEle);
            tEle.AppendChild(refEle);
            placeholderElement.ParentNode.ReplaceChild(rEle, placeholderElement);
        }

        private List<XmlElement> MatchImagePlaceholderElements()
        {
            var placeholderElements = new List<XmlElement>();
            var shapeElements = this.CompiledTemplateDocument.XmlDocument
                .SelectNodes("//" + WordMLDocument.ShapeElement, 
                    this.CompiledTemplateDocument.NamespaceManager);

            foreach (XmlElement ele in shapeElements)
            {
                var altAttr = ele.Attributes["alt"];
                if (altAttr != null && !string.IsNullOrWhiteSpace(altAttr.Value) && altAttr.Value.Trim()
                    .StartsWith("dtl://"))
                {
                    placeholderElements.Add(ele.ParentNode as XmlElement);
                }
            }

            return placeholderElements;
        }

        private void ProcessImagePlaceholderElement(XmlElement ele)
        {
            var nsmanager = this.CompiledTemplateDocument.NamespaceManager;
            var shapeEle = ele.SelectSingleNode("//" + WordMLDocument.ShapeElement, nsmanager);
            var binDataEle = ele.SelectSingleNode("//" + WordMLDocument.BinDataElement, nsmanager);
            var imageDataEle = shapeEle.SelectSingleNode("//" + WordMLDocument.ImageDataElement, nsmanager);
            var refExpr = shapeEle.Attributes["alt"].Value.Trim().Substring("dtl://".Length);
            var imageFormat = ImageFormatPattern.Value.Match(refExpr).Groups[1].Value;
            var id = Guid.NewGuid().ToString("N");
            var wordmlImageUrl = $"wordml://{id}.{imageFormat}";
            imageDataEle.Attributes["src"].Value = wordmlImageUrl;
            binDataEle.Attributes["w:name"].Value = wordmlImageUrl;
            binDataEle.InnerText = "{{" + refExpr + "}}";

            shapeEle.Attributes["alt"].Value = string.Empty;
        }


    }
}
