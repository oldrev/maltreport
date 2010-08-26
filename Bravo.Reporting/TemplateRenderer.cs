//作者：李维
//创建时间：2010-08-20

using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Collections;

using NVelocity.Runtime;
using NVelocity.App;
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

            var ctx = new VelocityContext();
            foreach (var pair in data)
            {
                ctx.Put(pair.Key, pair.Value);
            }

            var ve = new VelocityEngine();
            ve.Init();

            using (var inStream = odfResult.GetEntryInputStream(OdfDocument.ENTRY_CONTENT))
            using (var reader = new StreamReader(inStream, Encoding.UTF8))
            using (var ws = odfResult.GetEntryOutputStream(OdfDocument.ENTRY_CONTENT))
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

    }
}
