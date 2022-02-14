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

namespace Sandwych.Reporting.OfficeML
{
    public class ExcelMLTemplate : AbstractXmlTemplate<ExcelMLDocument>
    {
        private static readonly Lazy<Regex> ImageFormatPattern =
            new Lazy<Regex>(() => new Regex(@"^.*\|\s*image\s*:\s*'(.*)'\s*$", RegexOptions.Compiled | RegexOptions.CultureInvariant | RegexOptions.Singleline), true);

        public ExcelMLTemplate(ExcelMLDocument templateDocument) : base(templateDocument)
        {
        }

        protected override void PrepareTemplate()
        {
            this.ProcessPlaceholders();
        }

        public override async Task<ExcelMLDocument> RenderAsync(TemplateContext context)
        {
            var fluidContext = this.CreateFluidTemplateContext(null, context);
            var sb = new StringBuilder();
            using (var outputXmlWriter = new StringWriter(sb))
            {
                await this.TextTemplate.RenderAsync(outputXmlWriter, HtmlEncoder.Default, fluidContext);
            }
            return ExcelMLDocument.LoadFromText(sb.ToString());
        }

        private void ProcessPlaceholders()
        {
            var xml = this.TemplateDocument.XmlDocument;

            var workbookNode = FindFirstChildNode(xml, "Workbook");

            if (workbookNode == null)
            {
                throw new TemplateException("Invalid document format of Excel 2003 Xml");
            }

            var placeholders = FindAllPlaceholders(xml);

            foreach (XmlElement phe in placeholders)
            {
                var attr = phe.GetAttribute(ExcelMLDocument.HRefAttribute);
                var value = attr.Substring(5).Trim('/').Trim();

                if (value.Length < 2)
                {
                    throw new SyntaxErrorException();
                }

                if (value.Trim().StartsWith(WellknownConstants.DtlDirectiveChar))
                {
                    this.ProcessDirectiveTag(phe, value.Substring(1));
                }
                else if (value.Trim().StartsWith(WellknownConstants.DtlReferenceChar))
                {
                    this.ProcessReferenceTag(phe, value.Substring(1));
                }
                else
                {
                    throw new SyntaxErrorException(attr);
                }

            }

        }

        private void ProcessDirectiveTag(XmlElement placeholderNode, string value)
        {
            var directive = new DirectiveElement(this.TemplateDocument.XmlDocument, value);
            directive.ReduceTagByDirective(placeholderNode);
        }

        private static List<XmlElement> FindAllPlaceholders(XmlDocument doc)
        {
            Debug.Assert(doc != null);

            var placeholders = new List<XmlElement>(50);
            var allNodes = doc.GetElementsByTagName("Cell");

            foreach (XmlElement e in allNodes)
            {
                if (e.HasAttribute(ExcelMLDocument.HRefAttribute))
                {
                    var attr = e.GetAttribute(ExcelMLDocument.HRefAttribute);
                    if (attr.StartsWith("rtl://", StringComparison.Ordinal))
                    {
                        placeholders.Add(e);
                    }
                }
            }

            return placeholders;
        }

        private void ProcessReferenceTag(XmlElement phe, string value)
        {
            Debug.Assert(phe != null);
            Debug.Assert(!string.IsNullOrEmpty(value));

            phe.RemoveAttribute(ExcelMLDocument.HRefAttribute);
            phe.InnerText = value;
        }

        private static void ProcessDirectiveTag(XmlDocument xml, XmlElement phe, string value)
        {
            Debug.Assert(xml != null);
            Debug.Assert(phe != null);
            Debug.Assert(!string.IsNullOrEmpty(value));

            var se = new DirectiveElement(xml, value);
            if (phe.ParentNode.ChildNodes.Count == 1)
            {
                phe.ParentNode.ParentNode.ReplaceChild(se, phe.ParentNode);
            }
            else
            {
                phe.ParentNode.ReplaceChild(se, phe);
            }
        }

        private static XmlNode FindFirstChildNode(XmlNode parent, string childName)
        {
            Debug.Assert(parent != null);
            Debug.Assert(!string.IsNullOrEmpty(childName));
            foreach (XmlNode n in parent.ChildNodes)
            {
                if (n.Name == childName)
                {
                    return n;
                }
            }
            return null;
        }

    }
}
