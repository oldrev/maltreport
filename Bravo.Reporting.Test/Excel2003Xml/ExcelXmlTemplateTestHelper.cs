using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.IO;

using Bravo.Reporting.OpenDocument;

namespace Bravo.Reporting.Excel2003Xml.Test
{
    public static class ExcelXmlTemplateTestHelper
    {
        /// <summary>
        /// 一步执行模板编译、渲染并返回结果
        /// </summary>
        /// <param name="odfPath"></param>
        /// <returns></returns>
        public static ExcelXmlDocument RenderTemplate(string tmpPath, IDictionary<string, object> context)
        {
            var t = new ExcelXmlDocument();
            t.Load(tmpPath);
            return (ExcelXmlDocument)t.Compile().Render(context);
        }

        public static XmlDocument GetExcelXmlDocument(ExcelXmlDocument excelDoc)
        {
            var xmldoc = new XmlDocument();
            using(var ms = new MemoryStream(excelDoc.GetBuffer()))
            {
                xmldoc.Load(ms);
            }

            return xmldoc;
        }
    }
}
