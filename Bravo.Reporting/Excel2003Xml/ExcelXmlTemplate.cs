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

using NVelocity.Runtime;
using NVelocity.App;
using NVelocity.App.Events;
using NVelocity;
using NVelocity.Context;

namespace Bravo.Reporting.Excel2003Xml
{
    public class ExcelXmlTemplate : ExcelXmlDocument, ITemplate
    {
        private IDocument resultDocument;

        #region ITemplate 接口实现

        public IDocument Render(IDictionary<string, object> data)
        {
            this.resultDocument = new ExcelXmlDocument();
            this.CopyTo(this.resultDocument);

            var ctx = CreateVelocityContext(data);

            var ve = new VelocityEngine();
            ve.Init();

            //执行主渲染过程
            this.MainRender(ctx, ve);

            return this.resultDocument;
        }

        #endregion

        private void MainRender(VelocityContext ctx, VelocityEngine ve)
        {
            using (var inStream = this.resultDocument.GetEntryInputStream(this.resultDocument.MainContentEntryPath))
            using (var reader = new StreamReader(inStream, Encoding.UTF8))
            using (var ws = this.resultDocument.GetEntryOutputStream(this.resultDocument.MainContentEntryPath))
            using (var writer = new StreamWriter(ws))
            {
                //执行渲染
                var successed = ve.Evaluate(ctx, writer, "TemplateRender", reader);
                if (!successed)
                {
                    throw new TemplateException("Render template failed");
                }
                writer.Flush();
            }
        }

        private VelocityContext CreateVelocityContext(IDictionary<string, object> data)
        {
            var ctx = new VelocityContext();
            foreach (var pair in data)
            {
                ctx.Put(pair.Key, pair.Value);
            }

            ctx.Put(VelocityEscapeTool.DefaultKey, new VelocityEscapeTool());

            EventCartridge eventCart = new EventCartridge();
            eventCart.ReferenceInsertion += this.OnStringReferenceInsertion;
            ctx.AttachEventCartridge(eventCart);
            return ctx;
        }

        /// <summary>
        /// 此事件转义 XML 字符
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void OnStringReferenceInsertion(object sender, ReferenceInsertionEventArgs e)
        {
            var originalStr = e.OriginalValue as string;
            if (originalStr != null)
            {
                e.NewValue = SecurityElement.Escape(originalStr);
            }
        }
    }
}
