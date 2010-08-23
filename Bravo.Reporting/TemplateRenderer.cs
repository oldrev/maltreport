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
        public static OdfArchive Render(OdfArchive odfTemplate, IDictionary<string, object> data)
        {
            var odfResult = new OdfArchive();
            odfTemplate.CopyTo(odfResult);

            var ctx = new VelocityContext();
            foreach (var pair in data)
            {
                ctx.Put(pair.Key, pair.Value);
            }

            var ve = new VelocityEngine();
            ve.Init();

            using (var inStream = odfResult.GetContentInputStream(OdfArchive.ENTRY_CONTENT))
            using (var reader = new StreamReader(inStream, Encoding.UTF8))
            using (var ws = odfResult.GetContentOutputStream(OdfArchive.ENTRY_CONTENT))
            using (var writer = new StreamWriter(ws))
            {
                //执行渲染
                var successed = ve.Evaluate(ctx, writer, "OdfTemplateRender", reader);
                if (!successed)
                {
                    throw new TemplateException();
                }
                writer.Flush();
            }

            return odfResult;
        }

    }
}
