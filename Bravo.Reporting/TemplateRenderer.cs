//作者：李维
//创建时间：2010-08-20

using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Collections;
using System.Security;

using NVelocity.Runtime;
using NVelocity.App;
using NVelocity.App.Events;
using NVelocity;
using NVelocity.Context;

namespace Bravo.Reporting
{
    public static class TemplateRenderer
    {
        public static OdfDocument Render(OdfDocument odfTemplate, IDictionary<string, object> data)
        {
            var odfResult = new OdfDocument();
            odfTemplate.CopyTo(odfResult);

            var ctx = CreateVelocityContext(data);

            var ve = new VelocityEngine();
            ve.Init();

            using (var inStream = odfResult.GetEntryInputStream(OdfDocument.ContentEntry))
            using (var reader = new StreamReader(inStream, Encoding.UTF8))
            using (var ws = odfResult.GetEntryOutputStream(OdfDocument.ContentEntry))
            using (var writer = new StreamWriter(ws))
            {
                //执行渲染
                var successed = ve.Evaluate(ctx, writer, "OdfTemplateRender", reader);
                if (!successed)
                {
                    throw new TemplateException("Render template failed");
                }
                writer.Flush();
            }

            return odfResult;
        }

        private static VelocityContext CreateVelocityContext(IDictionary<string, object> data)
        {
            var ctx = new VelocityContext();
            foreach (var pair in data)
            {
                ctx.Put(pair.Key, pair.Value);
            }

            EventCartridge eventCart = new EventCartridge();
            eventCart.ReferenceInsertion +=
                new EventHandler<ReferenceInsertionEventArgs>(OnReferenceInsertion);
            ctx.AttachEventCartridge(eventCart);
            return ctx;
        }

        /// <summary>
        /// 此事件转义 XML 字符
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        static void OnReferenceInsertion(object sender, ReferenceInsertionEventArgs e)
        {
            var originalStr = e.OriginalValue as string;
            if (originalStr != null)
            {
                e.NewValue = SecurityElement.Escape(originalStr);
            }
        }

    }
}
