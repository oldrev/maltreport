using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

using Antlr.StringTemplate;

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
            st.RegisterAttributeRenderer(typeof(string), new SafeStringRenderer());

            foreach (var item in data)
            {
                st.SetAttribute(item.Key, item.Value);
            }

            using (var ws = odfResult.GetContentOutputStream(OdfArchive.ENTRY_CONTENT))
            using (var writer = new StreamWriter(ws))
            {
                st.Write(new NoIndentWriter(writer));
                writer.Flush();
            }

            return odfResult;
        }

    }
}
