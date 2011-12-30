//作者：李维
//创建时间：2010-09-03
using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;
using System.IO;
using System.Xml;

using Malt.Reporting.Xml;

namespace Malt.Reporting.OfficeXml
{
    public class ExcelMLTemplate : AbstractXmlBasedTemplate
    {
        public const string IndexAttribute = "ss:Index";
        public const string ExpandedColumnCountAttribute = "ss:ExpandedColumnCount";
        public const string ExpandedRowCountAttribute = "ss:ExpandedRowCount";
        public const string FormatAttribute = "ss:Format";
        public const string TypeAttribute = "ss:Type";
        private ITextTemplateEngine engine;

        public ExcelMLTemplate()
        {
            this.engine = new VelocityTextTemplateEngine("ExcelXmlTemplate");
            this.engine.RegisterFilter(typeof(string), new XmlStringRenderFilter());
        }

        public override void Compile()
        {
            ExcelMLCompiler.Compile(this);
        }

        #region IDocument 成员

        #endregion


        #region ITemplate 接口实现

        public override IDocument Render(IDictionary<string, object> context)
        {
            Debug.Assert(context != null);
            Debug.Assert(this.engine != null);

            if (context == null)
            {
                throw new ArgumentNullException("context");
            }

            //执行主要内容的渲染过程
            using (var inStream = new MemoryStream(base.GetBuffer(), false))
            using (var reader = new StreamReader(inStream, Encoding.UTF8))
            using (var ws = new MemoryStream())
            using (var writer = new StreamWriter(ws))
            {
                //执行渲染
                this.engine.Evaluate(context, reader, writer);
                writer.Flush();
                ws.Flush();
                var resultDoc = new ExcelMLTemplate();
                resultDoc.PutBuffer(ws.ToArray());
                return resultDoc;
            }
        }

        #endregion

        internal void LoadFromDocument(ExcelMLTemplate doc)
        {
            Debug.Assert(doc != null);

            var buf = doc.GetBuffer();
            var newBuf = new byte[buf.Length];
            Buffer.BlockCopy(buf, 0, newBuf, 0, buf.Length);
            this.PutBuffer(newBuf);
        }


    }
}
