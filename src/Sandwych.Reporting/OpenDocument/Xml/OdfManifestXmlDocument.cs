using System;
using System.Globalization;
using System.IO;
using System.Xml;

namespace Sandwych.Reporting.OpenDocument.Xml
{
    public class OdfManifestXmlDocument : AbstractOdfXmlDocument
    {
        public const string PicturesFullPath = @"Pictures/";

        private XmlElement manifestElement;

        public OdfManifestXmlDocument(Stream stream) : base(stream)
        {
            this.manifestElement = (XmlElement)this.SelectSingleNode(@"/manifest:manifest", this.NamespaceManager);

            if (this.manifestElement == null)
            {
                throw new TemplateException("Invalid OpenDocument manifest");
            }
        }

        /// <summary>
        /// 创建图片目录条目
        /// </summary>
        public void CreatePicturesEntryElement()
        {
            //看看是否有 "Pictures/" 这一项
            var xpath = string.Format(
                CultureInfo.InvariantCulture,
                @"/manifest:manifest/manifest:file-entry[@manifest:full-path=""{0}""]", PicturesFullPath);
            var picturesEntryNode = this.SelectSingleNode(xpath, this.NamespaceManager);

            if (picturesEntryNode == null)
            {
                var picturesEntryElement = this.CreateElement(
                    "manifest", "file-entry", OdfNamespaceManager.ManifestNamespace);
                picturesEntryElement.SetAttribute("media-type", OdfNamespaceManager.ManifestNamespace, string.Empty);
                picturesEntryElement.SetAttribute("full-path", OdfNamespaceManager.ManifestNamespace, PicturesFullPath);
                manifestElement.AppendChild(picturesEntryElement);
            }
        }

        /// <summary>
        /// 添加一个文件条目
        /// </summary>
        /// <param name="extensionName"></param>
        /// <param name="fullPath"></param>
        public void AppendFileEntry(string mediaType, string fullPath)
        {
            if (string.IsNullOrEmpty(mediaType))
            {
                throw new ArgumentNullException("mediaType");
            }

            if (string.IsNullOrEmpty(fullPath))
            {
                throw new ArgumentNullException("fullPath");
            }

            var fileEntryElement = this.CreateElement(
             "manifest", "file-entry", OdfNamespaceManager.ManifestNamespace);
            fileEntryElement.SetAttribute("full-path", OdfNamespaceManager.ManifestNamespace, fullPath);
            fileEntryElement.SetAttribute("media-type", OdfNamespaceManager.ManifestNamespace, mediaType);

            manifestElement.AppendChild(fileEntryElement);
        }

        /// <summary>
        /// 添加一个图片文件条目
        /// </summary>
        /// <param name="extensionName"></param>
        /// <param name="fullPath"></param>
        public void AppendImageFileEntry(string extensionName, string fullPath)
        {
            var mediaType = @"image/" + extensionName.ToLowerInvariant();
            this.AppendFileEntry(mediaType, fullPath);
        }
    }
}