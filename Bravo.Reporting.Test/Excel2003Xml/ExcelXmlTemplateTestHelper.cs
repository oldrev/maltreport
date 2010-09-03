using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;

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
        public static IDocument RenderTemplate(string tmpPath, IDictionary<string, object> context)
        {
            var t = new ExcelXmlDocument();
            t.Load(tmpPath);
            var compiler = new ExcelXmlTemplateCompiler();
            var template = compiler.Compile(t);
            return template.Render(context);
        }
    }
}
