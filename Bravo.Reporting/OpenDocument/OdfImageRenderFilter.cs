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
            var image = (Image)originalValue;

            string filename = this.PutImage(image);

            return GetDrawImageElementXml(filename);

        }

        private static object GetDrawImageElementXml(string imagePath)
        {
            using (var ws = new StringWriter(CultureInfo.InvariantCulture))
            using (var xw = new XmlTextWriter(ws))
            {
                WriteDrawImageElement(xw, imagePath);
                xw.Flush();

                return ws.ToString();
            }
        }

        private static void WriteDrawImageElement(XmlWriter xw, string imagePath)
        {
            Debug.Assert(!string.IsNullOrEmpty(imagePath));
            Debug.Assert(xw != null);

            xw.WriteStartElement("draw:image");
            xw.WriteAttributeString("xlink:href", imagePath);
            xw.WriteAttributeString("xlink:type", "simple");
            xw.WriteAttributeString("xlink:show", "embed");
            xw.WriteAttributeString("xlink:actuate", "onLoad");
            xw.WriteEndElement();
        }

        #endregion

        private string PutImage(Image image)
        {
            Debug.Assert(image != null);

            string filename = null;
            var hasImage = this.userImages.TryGetValue(image, out filename);

            if(!hasImage)
            {
                filename = this.resultDocument.AddImage(image);
                this.userImages[image] = filename;
            }

            return filename;
        }

    }
}
