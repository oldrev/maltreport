using System;
using System.Globalization;
using System.IO;
using System.Xml;

namespace Sandwych.Reporting.Odf.Xml
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
            var picturesEntryNode = this.GetFileEntryNodeOrDefaultByFullPath(PicturesFullPath);

            if (picturesEntryNode == null)
            {
                var picturesEntryElement = this.CreateElement(
                    "manifest", "file-entry", OdfNamespaceManager.ManifestNamespace);
                picturesEntryElement.SetAttribute("media-type", OdfNamespaceManager.ManifestNamespace, string.Empty);
                picturesEntryElement.SetAttribute("full-path", OdfNamespaceManager.ManifestNamespace, PicturesFullPath);
                manifestElement.AppendChild(picturesEntryElement);
            }
        }

        public void RemoveFileEntry(string fullPath)
        {
            if (fullPath == null)
            {
                throw new ArgumentNullException(nameof(fullPath));
            }
            var node = this.GetFileEntryNodeOrDefaultByFullPath(fullPath);
            if(node == null)
            {
                throw new FileNotFoundException(fullPath);
            }
            node.ParentNode.RemoveChild(node);
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
        /// Add a image entry to Odf's manifest
        /// </summary>
        /// <param name="fullPath"></param>
        /// <param name="extensionName"></param>
        public void AppendImageFileEntry(string fullPath, string extensionName)
        {
            var mediaType = @"image/" + extensionName.ToLowerInvariant();
            this.AppendFileEntry(mediaType, fullPath);
        }

        private XmlNode GetFileEntryNodeOrDefaultByFullPath(string fullPath)
        {

            var xpath = string.Format(
                CultureInfo.InvariantCulture,
                @"/manifest:manifest/manifest:file-entry[@manifest:full-path=""{0}""]", fullPath);
            return this.SelectSingleNode(xpath, this.NamespaceManager);
        }

    }
}