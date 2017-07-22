using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.IO;
using System.Globalization;

namespace Sandwych.Reporting.OpenDocument
{
    internal class OdfManifestDocument : XmlDocument
    {
        public const string PicturesFullPath = @"Pictures/";
        private OdfNamespaceManager nsmanager;
        private XmlElement manifestElement;

        public override void Load(Stream inStream)
        {
            base.Load(inStream);
            this.Init();
        }

        public override void Load(TextReader txtReader)
        {
            base.Load(txtReader);
            this.Init();
        }

        public override void Load(XmlReader reader)
        {
            base.Load(reader);
            this.Init();
        }

        public override void LoadXml(string xml)
        {
            base.LoadXml(xml);
            this.Init();
        }

        private void Init()
        {
            this.nsmanager = new OdfNamespaceManager(this.NameTable);
            this.nsmanager.LoadOpenDocumentNamespaces();

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
                CultureInfo.InvariantCulture,
                @"/manifest:manifest/manifest:file-entry[@manifest:full-path=""{0}""]", PicturesFullPath);
            var picturesEntryNode = this.SelectSingleNode(xpath, nsmanager);

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
