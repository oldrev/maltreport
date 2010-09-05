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

namespace Bravo.Reporting.OpenDocument
{
    public class OdfTemplate : OdfDocument, ITemplate
    {
        private IDictionary<Image, string> userImages
            = new Dictionary<Image, string>();

        private IDocument resultDocument;

        private ITextTemplateEngine engine;

        private void CreateTextEngine()
        {
            this.engine = new VelocityTextTemplateEngine("OdfTemplate");
            this.engine.RegisterFilter(typeof(string), new XmlStringRenderFilter());
            this.engine.RegisterFilter(typeof(Image), new OdfImageRenderFilter(this.userImages, this.resultDocument));
        }

        #region ITemplate 接口实现

        public IDocument Render(IDictionary<string, object> context)
        {
            this.resultDocument = new OdfDocument();
            this.CopyTo(this.resultDocument);

            this.CreateTextEngine();

            using (var inStream = this.resultDocument.GetEntryInputStream(this.resultDocument.MainContentEntryPath))
            using (var reader = new StreamReader(inStream, Encoding.UTF8))
            using (var ws = this.resultDocument.GetEntryOutputStream(this.resultDocument.MainContentEntryPath))
            using (var writer = new StreamWriter(ws))
            {
                //执行渲染
                this.engine.Evaluate(context, reader, writer);
            }

            return this.resultDocument;
        }

        #endregion

    }
}
