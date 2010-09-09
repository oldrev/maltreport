using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.IO;

using Bravo.Reporting.OpenDocument;

namespace Bravo.Reporting.Test
{
    public static class TemplateTestHelper
    {
        /// <summary>
        /// 一步执行模板编译、渲染并返回结果
        /// </summary>
        /// <param name="odfPath"></param>
        /// <returns></returns>
        public static T RenderTemplate<T>(string tmpPath, IDictionary<string, object> context)
            where T : IDocument, new()
        {
            var t = new T();
            t.Load(tmpPath);
            return (T)t.Compile().Render(context);
        }

        public static XmlDocument GetlXmlDocument<T>(T singleXmlDoc)
            where T : IDocument
        {
            var xmldoc = new XmlDocument();
            using (var ms = new MemoryStream(singleXmlDoc.GetBuffer()))
            {
                xmldoc.Load(ms);
            }

            return xmldoc;
        }
    }
}
