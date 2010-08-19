using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;

namespace Bravo.Reporting
{
    public class OdtTemplateCompiler : ITemplateCompiler
    {
        #region ITemplateCompiler 成员

        public OdfArchive Compile(OdfArchive inputOdf)
        {

            var odfTemplate = new OdfArchive();
            inputOdf.CopyTo(odfTemplate);

            var xml = LoadXml(odfTemplate);
            var nsmanager = new XmlNamespaceManager(xml.NameTable);
            nsmanager.AddNamespace("text", "urn:oasis:names:tc:opendocument:xmlns:text:1.0");

            //这里把 ODF 模板转换为  StringTemplate
            var placeholders = xml.SelectNodes("//text:placeholder", nsmanager);
            foreach (XmlNode placeholder in placeholders)
            {
                var value = placeholder.InnerText.Trim();
                if (value.Length >= 2 && value[0] == '<')
                {
                    value = value.Substring(1, value.Length - 2).Trim();
                }

                if (value.Length > 0)
                {
                    XmlNode parent = placeholder.ParentNode;
                    var textNode = xml.CreateNode(XmlNodeType.Element, "text:span", "text");
                    textNode.InnerText = "$" + value + "$";
                    parent.ReplaceChild(textNode, placeholder);
                }
            }

            SaveXml(odfTemplate, xml);

            return odfTemplate;
        }

        private static void SaveXml(OdfArchive odfTemplate, XmlDocument xml)
        {
            using (var cos = odfTemplate.GetContentOutputStream(OdfArchive.ENTRY_CONTENT))
            using (var writer = new XmlTextWriter(cos, Encoding.UTF8))
            {
                xml.WriteTo(writer);
            }
        }

        private static XmlDocument LoadXml(OdfArchive odfTemplate)
        {
            var xml = new XmlDocument();
            using (var contentStream = odfTemplate.GetContentInputStream(OdfArchive.ENTRY_CONTENT))
            {
                xml.Load(contentStream);
            }
            return xml;
        }

        #endregion
    }
}
