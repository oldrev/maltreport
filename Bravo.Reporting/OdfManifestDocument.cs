using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.IO;

namespace Bravo.Reporting
{
    /// <summary>
    /// 
    /// </summary>
    internal class OdfManifestDocument : XmlDocument
    {
        public const string PicturesFullPath = @"Pictures/";
        public const string ManifestNamespace = @"urn:oasis:names:tc:opendocument:xmlns:manifest:1.0";

        private XmlNamespaceManager nsmanager;
        private XmlElement manifestElement;

        public OdfManifestDocument(Stream inStream)
        {
            this.Load(inStream);
            this.nsmanager = new XmlNamespaceManager(this.NameTable);
            nsmanager.AddNamespace("manifest", ManifestNamespace);
            this.manifestElement = (XmlElement)this.SelectSingleNode(@"/manifest:manifest", nsmanager);

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
                @"/manifest:manifest/manifest:file-entry[@manifest:full-path=""{0}""]", PicturesFullPath);
            var picturesEntryNode = this.SelectSingleNode(xpath, nsmanager);

            if (picturesEntryNode == null)
            {
                var picturesEntryElement = this.CreateElement(
                    "manifest", "file-entry", ManifestNamespace);
                picturesEntryElement.SetAttribute("media-type", ManifestNamespace, string.Empty);
                picturesEntryElement.SetAttribute("full-path", ManifestNamespace, PicturesFullPath);
                manifestElement.AppendChild(picturesEntryElement);
            }
        }

        /// <summary>
        /// 添加一个文件条目
        /// </summary>
        /// <param name="extensionName"></param>
        /// <param name="fullPath"></param>
        public void AppendFileEntry(string extensionName, string fullPath)
        {
            var fileEntryElement = this.CreateElement(
             "manifest", "file-entry", ManifestNamespace);
            var mediaType = string.Format("image/{0}", extensionName.ToLower());
            fileEntryElement.SetAttribute("media-type", ManifestNamespace, mediaType);
            fileEntryElement.SetAttribute("full-path", ManifestNamespace, fullPath);

            manifestElement.AppendChild(fileEntryElement);
        }
    }
}
