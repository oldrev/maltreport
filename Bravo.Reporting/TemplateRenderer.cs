using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

using Antlr3.ST;

namespace Bravo.Reporting
{
    public static class TemplateRenderer
    {
        public static OdfArchive Render(OdfArchive odfTemplate, IDictionary<string, object> data)
        {
            var odfResult = new OdfArchive();
            odfTemplate.CopyTo(odfResult);

            string content;
            using (var inStream = odfResult.GetContentInputStream(OdfArchive.ENTRY_CONTENT))
            using (var reader = new StreamReader(inStream, Encoding.UTF8))
            {
                content = reader.ReadToEnd();
            }

            var st = new StringTemplate(content);
            st.RegisterRenderer(typeof(string), new SafeStringRenderer());

            foreach (var item in data)
            {
                st.SetAttribute(item.Key, item.Value);
            }

            //执行转换
            var renderedContent = st.ToString();

            using (var ws = odfResult.GetContentOutputStream(OdfArchive.ENTRY_CONTENT))
            using (var writer = new StreamWriter(ws))
            {
                writer.Write(renderedContent);
            }

            return odfResult;
        }

    }
}
