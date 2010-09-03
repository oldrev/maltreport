using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;
using System.Xml;
using System.IO;
using System.Globalization;

namespace Bravo.Reporting.OpenDocument
{
    internal class OdfImageRenderFilter : IRenderFilter
    {
        private IDictionary<Image, string> userImages;
        private IDocument resultDocument;

        public OdfImageRenderFilter(IDictionary<Image, string> userImages, IDocument odfDoc)
        {
            Debug.Assert(userImages != null);
            Debug.Assert(odfDoc != null);

            this.userImages = userImages;
            this.resultDocument = odfDoc;
        }

        #region IRenderFilter 成员

        public object Filter(object originalValue)
        {
            var image = originalValue as Image;
            Debug.Assert(image != null);

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

                return ws.ToString();
            }

        }

        #endregion
    }
}
