using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;

using NUnit.Framework;

using Bravo.Reporting.Test;

namespace Bravo.Reporting.OpenDocument.Test
{
    public static class OdfTemplateTestHelper
    {

        public const string OpenDocumentRngFile =
            @"resources/schemas/opendocument/OpenDocument-v1.2-cd05-schema.rng";

        /// <summary>
        /// 一部执行模板编译、渲染并返回结果
        /// </summary>
        /// <param name="odfPath"></param>
        /// <returns></returns>
        public static OdfDocument RenderTemplate(string odfPath, IDictionary<string, object> context)
        {
            var odf = new OdfDocument();
            odf.Load(odfPath);
            return (OdfDocument)odf.Compile().Render(context);
        }

        public static XmlDocument GetContentDocument(OdfDocument odfTemplate)
        {
            var xmldoc = new XmlDocument();
            using (var inputStream = odfTemplate.GetEntryInputStream(odfTemplate.MainContentEntryPath))
            {
                xmldoc.Load(inputStream);
            }
            return xmldoc;
        }

        public static void AssertOpenDocumentContentWellFormed(OdfDocument odf)
        {
            using (var inStream = odf.GetEntryInputStream(odf.MainContentEntryPath))
            {
                var validateResult = TemplateTestHelper.RelaxngValidate(inStream, OpenDocumentRngFile);
                Assert.IsTrue(validateResult);
            }
        }
    }
}
