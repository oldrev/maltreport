//作者：李维
//创建时间：2010-09-03

using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Collections;
using System.Security;
using System.Xml;
using System.Diagnostics;
using System.Globalization;

using Bravo.Reporting.Xml;

namespace Bravo.Reporting.Excel2003Xml
{
    public class ExcelXmlTemplate : ExcelXmlDocument, ITemplate
    {
        private ITextTemplateEngine engine;

        public ExcelXmlTemplate()
        {
            this.engine = new VelocityTextTemplateEngine("ExcelXmlTemplate");
            this.engine.RegisterFilter(typeof(string), new XmlStringRenderFilter());
        }

        #region ITemplate 接口实现

        public IDocument Render(IDictionary<string, object> context)
        {
            Debug.Assert(this.engine != null);

            var resultDocument = new ExcelXmlDocument();
            this.CopyTo(resultDocument);

            //执行主要内容的渲染过程
            using (var inStream = resultDocument.GetEntryInputStream(resultDocument.MainContentEntryPath))
            using (var reader = new StreamReader(inStream, Encoding.UTF8))
            using (var ws = resultDocument.GetEntryOutputStream(resultDocument.MainContentEntryPath))
            using (var writer = new StreamWriter(ws))
            {
                //执行渲染
                this.engine.Evaluate(context, reader, writer);
            }

            return resultDocument;
        }

        #endregion

    }
}
