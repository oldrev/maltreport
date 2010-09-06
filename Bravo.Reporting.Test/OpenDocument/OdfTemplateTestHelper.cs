using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;

using Bravo.Reporting.OpenDocument;

namespace Bravo.Reporting.OpenDocument.Test
{
    public static class OdfTemplateTestHelper
    {
        /// <summary>
        /// 一部执行模板编译、渲染并返回结果
        /// </summary>
        /// <param name="odfPath"></param>
        /// <returns></returns>
        public static IDocument RenderTemplate(string odfPath, IDictionary<string, object> context)
        {
            var odf = new OdfDocument();
            odf.Load(odfPath);
            return odf.Compile().Render(context);
        }

        public static XmlDocument GetContentDocument(IDocument odfTemplate)
        {
            var xmldoc = new XmlDocument();
            using (var inputStream = odfTemplate.GetEntryInputStream(odfTemplate.MainContentEntryPath))
            {
                xmldoc.Load(inputStream);
            }
            return xmldoc;
        }
    }
}
