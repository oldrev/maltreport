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
using System.Text.Encodings.Web;

namespace Sandwych.Reporting.OfficeML
{
    public class WordMLTemplate : AbstractTemplate<WordMLDocument>
    {
        private IFluidTemplate _fluidTemplate;

        public WordMLTemplate(WordMLDocument templateDocument) : base(templateDocument)
        {
        }

        private string GetCompiledMainContent()
        {
            using (var sw = new StringWriter())
            using (var writer = XmlWriter.Create(sw))
            {
                this.TemplateDocument.XmlDocument.WriteTo(writer);
                return sw.ToString();
            }
        }

        protected override void CompileAndParse()
        {
            // this.ProcessPlaceholders();
            var stringTemplate = this.GetCompiledMainContent();

            if (!FluidTemplate.TryParse(stringTemplate, out _fluidTemplate, out var errors))
            {
                throw new SyntaxErrorException(errors.Aggregate((x, y) => x + "\n" + y));
            }
        }


        public override async Task<WordMLDocument> RenderAsync(TemplateContext context)
        {
            using (var outputXmlWriter = new StringWriter())
            {
                await _fluidTemplate.RenderAsync(outputXmlWriter, HtmlEncoder.Default, context.FluidContext);
                return WordMLDocument.LoadXml(outputXmlWriter.ToString());
            }
        }

        public override WordMLDocument Render(TemplateContext context) =>
            this.RenderAsync(context).GetAwaiter().GetResult();


        private void ProcessPlaceholders()
        {
            var placeholders = this.FindAllPlaceholders();

            foreach (XmlElement phe in placeholders)
            {
                var attr = phe.GetAttribute(WordMLDocument.DestAttribute);
                var value = attr.Substring(WellknownConstants.DtlProtocolPrefix.Length).Trim('/', ' ');

                value = UrlUtility.UrlDecode(value, Encoding.UTF8);

                if (value.Length < 2)
                {
                    throw new SyntaxErrorException();
                }

                if (value[0] == '#')
                {
                    this.ProcessDirectiveTag(phe, value.Substring(1));
                }
                else if (value[0] == '$')
                {
                    this.ProcessReferenceTag(phe, value.Substring(1));
                }
                else
                {
                    throw new SyntaxErrorException($"Syntax error: '{attr}'");
                }

            }
        }

        private void ProcessDirectiveTag(XmlElement placeholderNode, string value)
        {
            var directive = new DirectiveElement(this.TemplateDocument.XmlDocument, value);
            directive.ReduceTagByDirective(placeholderNode);
        }

        private List<XmlElement> FindAllPlaceholders()
        {
            var placeholders = new List<XmlElement>();
            var allNodes = this.TemplateDocument.XmlDocument.GetElementsByTagName(WordMLDocument.HlinkElement);

            foreach (XmlElement e in allNodes)
            {
                var attr = e.GetAttribute(WordMLDocument.DestAttribute);
                if (attr.StartsWith(WellknownConstants.DtlProtocolPrefix))
                {
                    placeholders.Add(e);
                }
            }

            return placeholders;
        }

        private void ProcessReferenceTag(XmlElement placeholderElement, string value)
        {
            var xml = this.TemplateDocument.XmlDocument;
            var refEle = new ReferenceElement(xml, value);
            var rEle = xml.CreateElement("w:r", WordMLNamespaceManager.WNamespace);
            var tEle = xml.CreateElement("w:t", WordMLNamespaceManager.WNamespace);
            rEle.AppendChild(tEle);
            tEle.AppendChild(refEle);
            placeholderElement.ParentNode.ReplaceChild(rEle, placeholderElement);
        }


    }
}
