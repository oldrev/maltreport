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
            var xml = new XmlDocument();

            var odfTemplate = new OdfArchive();
            inputOdf.CopyTo(odfTemplate);

            /*

            using (var contentStream = odfTemplate.GetContentInputStream(OdfArchive.ENTRY_CONTENT))
            {
                xml.Load(contentStream);
            }

            //这里把 ODF 模板转换为  StringTemplate
            //xmxsdsdsdsd

            using (var cos = odfTemplate.GetContentOutputStream(OdfArchive.ENTRY_CONTENT))
            using (var writer = new XmlTextWriter(cos, Encoding.UTF8))
            {
                xml.WriteTo(writer);
            }
             */

            return odfTemplate;
        }

        #endregion
    }
}
