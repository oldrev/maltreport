//作者：李维
//创建时间：2010-08-20

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

namespace Bravo.Reporting.OpenDocument
{
    public class OdfTemplate : OdfDocument, ITemplate
    {
        private ITextTemplateEngine engine ;

        public OdfTemplate()
        {
            this.engine = new VelocityTextTemplateEngine("OdfTemplate");
        }

        private void ResetTextEngine(IDictionary<Image, string> userImages, IDocument resultDocument)
        {
            Debug.Assert(this.engine != null);
            Debug.Assert(userImages != null);
            Debug.Assert(resultDocument != null);

            this.engine.Reset();
            this.engine.RegisterFilter(typeof(string), new XmlStringRenderFilter());
            this.engine.RegisterFilter(typeof(Image), new OdfImageRenderFilter(userImages, resultDocument));
        }

        #region ITemplate 接口实现

        public IDocument Render(IDictionary<string, object> context)
        {
            Debug.Assert(this.engine != null);

            if (context == null)
            {
                throw new ArgumentNullException("context");
            }

            var userImages = new Dictionary<Image, string>();
            var resultDocument = new OdfDocument();
            this.CopyTo(resultDocument);

            this.ResetTextEngine(userImages, resultDocument);

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
