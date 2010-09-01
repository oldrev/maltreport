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

using NVelocity.Runtime;
using NVelocity.App;
using NVelocity.App.Events;
using NVelocity;
using NVelocity.Context;

namespace Bravo.Reporting
{
    public class TemplateRenderer
    {

        private IDictionary<Image, string> userImages
            = new Dictionary<Image, string>();

        private OdfDocument templateDocument;
        private OdfDocument resultDocument;

        public TemplateRenderer(OdfDocument template)
        {
            if (template == null)
            {
                throw new ArgumentNullException("template");
            }

            this.templateDocument = template;

        }

        public OdfDocument Render(IDictionary<string, object> data)
        {
            this.resultDocument = new OdfDocument();
            this.templateDocument.CopyTo(this.resultDocument);

            var ctx = CreateVelocityContext(data);

            var ve = new VelocityEngine();
            ve.Init();

            //执行主渲染过程
            this.MainRender(ctx, ve);

            //处理 Manifest.xml 文件
            if (this.userImages.Count > 0)
            {
                this.ProcessManifest();
            }

            return this.resultDocument;
        }

        private void MainRender(VelocityContext ctx, VelocityEngine ve)
        {
            using (var inStream = this.resultDocument.GetEntryInputStream(OdfDocument.ContentEntry))
            using (var reader = new StreamReader(inStream, Encoding.UTF8))
            using (var ws = this.resultDocument.GetEntryOutputStream(OdfDocument.ContentEntry))
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
        }

        private void ProcessManifest()
        {
            OdfManifestDocument manifestDoc = null;

            using (var manifestStream = this.templateDocument.GetEntryInputStream(OdfDocument.ManifestEntry))
            {
                manifestDoc = new OdfManifestDocument();
                manifestDoc.Load(manifestStream);
            }

            if (this.userImages.Count > 0)
            {
                manifestDoc.CreatePicturesEntryElement();
            }

            foreach (var item in this.userImages)
            {
                manifestDoc.AppendFileEntry(item.Key.ExtensionName, item.Value);
            }

            //处理 manifest.xml
            using (var manifestStream = this.resultDocument.GetEntryOutputStream(OdfDocument.ManifestEntry))
            {
                manifestDoc.Save(manifestStream);
            }
        }


        private VelocityContext CreateVelocityContext(IDictionary<string, object> data)
        {
            var ctx = new VelocityContext();
            foreach (var pair in data)
            {
                ctx.Put(pair.Key, pair.Value);
            }

            EventCartridge eventCart = new EventCartridge();
            eventCart.ReferenceInsertion += this.OnStringReferenceInsertion;
            eventCart.ReferenceInsertion += this.OnImageReferenceInsertion;
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

        /// <summary>
        /// 此事件处理 Image 数据
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void OnImageReferenceInsertion(object sender, ReferenceInsertionEventArgs e)
        {
            var image = e.OriginalValue as Image;
            if (image != null)
            {
                string filename = null;

                if (this.userImages.ContainsKey(image))
                {
                    filename = this.userImages[image];
                }
                else
                {
                    filename = this.resultDocument.AddImage(image);
                    this.userImages[image] = filename;
                }

                using (var ws = new StringWriter(CultureInfo.InvariantCulture))
                using (var xw = new XmlTextWriter(ws))
                {
                    xw.WriteStartElement("draw:image");
                    xw.WriteAttributeString("xlink:href", filename);
                    xw.WriteAttributeString("xlink:type", "simple");
                    xw.WriteAttributeString("xlink:show", "embed");
                    xw.WriteAttributeString("xlink:actuate", "onLoad");
                    xw.WriteEndElement();
                    xw.Flush();

                    e.NewValue = ws.ToString();
                }

            }

        }

    }
}
