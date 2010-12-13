//作者：李维
//创建时间：2010-08-20
using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Diagnostics;
using System.Xml;
using System.Text.RegularExpressions;

using Ionic.Zip;

using Bravo.Reporting.Xml;

namespace Bravo.Reporting.OpenDocument
{
    public class OdfDocument : ZipArchiveDocumentBase
    {
        internal const string MimeTypeEntryPath = "mimetype";
        internal const string SettingsEntryPath = "settings.xml";
        public const string ManifestEntryPath = "META-INF/manifest.xml";

        private const string TextPlaceholderElement = @"text:placeholder";
        private const string DrawTextBoxElement = @"draw:text-box";
        private const string TextAnchorElement = @"text:a";
        private const string TextPlaceholderTypeAttribute = @"text:placeholder-type";
        private const string TableRowElement = @"table:table-row";

        public static readonly Regex PlaceHolderValuePattern =
            new Regex(@"^<\s*(.*)\s*>$");

        public static readonly Regex HyperLinkValuePattern =
            new Regex(@"^rtl://(.*)\s*$");

        /// <summary>
        /// 加载到内存的 ODF 的文件内容
        /// </summary>
        private IDictionary<string, byte[]> entries = new Dictionary<string, byte[]>();

        public OdfDocument()
        {
        }

        public override void Load(Stream inStream)
        {
            if (inStream == null)
            {
                throw new ArgumentNullException("inStream");
            }

            this.entries.Clear();

            //把 zip 的内容加载到内存
            using (var zf = ZipFile.Read(inStream))
            {
                foreach (ZipEntry ze in zf)
                {
                    using (var ws = this.GetEntryOutputStream(ze.FileName))
                    {
                        ze.Extract(ws);
                    }
                }
            }
        }

        public override void Save(Stream outStream)
        {
            if (outStream == null || !outStream.CanWrite)
            {
                throw new ArgumentNullException("outStream");
            }

            //ODF 格式约定 mimetype 必须为第一个文件
            if (!this.entries.ContainsKey(MimeTypeEntryPath))
            {
                throw new InvalidDataException("Can not found entry: 'mimetype'");
            }

            using (var ze = new ZipFile())
            {
                this.AppendZipEntry(ze, MimeTypeEntryPath);

                foreach (var item in this.entries)
                {
                    if (item.Key == MimeTypeEntryPath)
                    {
                        continue;
                    }

                    this.AppendZipEntry(ze, item.Key);
                }

                ze.Save(outStream);
            }
        }

        private void AppendZipEntry(ZipFile zf, string name)
        {
            Debug.Assert(zf != null);
            Debug.Assert(!string.IsNullOrEmpty(name));
            Debug.Assert(this.entries.ContainsKey(name));

            var data = this.entries[name];
            var ze = zf.AddEntry(name, data);

            var extensionName = Path.GetExtension(name).ToUpperInvariant();

            switch (extensionName)
            {
                case "JPEG":
                case "JPG":
                case "PNG":
                    ze.CompressionMethod = CompressionMethod.None;
                    break;

                default:
                    ze.CompressionMethod = CompressionMethod.Deflate;
                    ze.CompressionLevel = Ionic.Zlib.CompressionLevel.BestCompression;
                    break;
            }
        }

        public override ICollection<string> EntryPaths
        {
            get { return this.entries.Keys; }
        }

        public override Stream GetEntryInputStream(string entryPath)
        {
            if (string.IsNullOrEmpty(entryPath))
            {
                throw new ArgumentNullException("entryPath");
            }

            var data = this.entries[entryPath];
            return new MemoryStream(data);
        }

        /// <summary>
        /// 千万要记得关闭
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public override Stream GetEntryOutputStream(string entryPath)
        {
            if (string.IsNullOrEmpty(entryPath))
            {
                throw new ArgumentNullException("entryPath");
            }

            var oms = new OutputMemoryStream(entryPath, this.entries);
            this.entries[entryPath] = oms.ToArray();

            return oms;
        }

        public override bool EntryExists(string entryPath)
        {
            if (string.IsNullOrEmpty(entryPath))
            {
                throw new ArgumentNullException("entryPath");
            }
            return this.entries.ContainsKey(entryPath);
        }

        public override string AddImage(Image img)
        {
            if (img == null)
            {
                throw new ArgumentNullException("img");
            }

            var fullPath = "Pictures/" + img.DocumentFileName;
            using (var outStream = this.GetEntryOutputStream(fullPath))
            {
                outStream.Write(img.GetData(), 0, img.DataSize);
            }

            var manifestDoc = new OdfManifestDocument();
            using (var manifestInStream = this.GetEntryInputStream(OdfDocument.ManifestEntryPath))
            {
                manifestDoc.Load(manifestInStream);
            }

            manifestDoc.AppendImageFileEntry(img.ExtensionName, fullPath);
            manifestDoc.CreatePicturesEntryElement();

            using (var manifestOutStream = this.GetEntryOutputStream(OdfDocument.ManifestEntryPath))
            {
                manifestDoc.Save(manifestOutStream);
            }

            return fullPath;
        }

        public override string MainContentEntryPath { get { return "content.xml"; } }

        public override ITemplate Compile()
        {
            var t = new OdfTemplate();
            this.CopyTo(t);

            var xml = new XmlDocument();
            t.ReadMainContentXml(xml);
            var nsmanager = new OdfNamespaceManager(xml.NameTable);
            nsmanager.LoadOpenDocumentNamespaces();

            //第1遍，先处理简单的Tag 替换
            PreprocessElements(xml, nsmanager);

            //第2遍，处理表格循环
            ProcessTableRows(xml, nsmanager);

            WriteXmlContent(t, xml);

            return t;
        }

        #region ICloneable 接口
        public override object Clone()
        {
            var destDoc = new OdfDocument();
            this.CopyTo(destDoc);
            return destDoc;
        }
        #endregion


        #region Compiler Methods
        private static void WriteXmlContent(OdfTemplate t, XmlDocument xml)
        {
            //把编译后的 XmlDocument 写入
            using (var cos = t.GetEntryOutputStream(t.MainContentEntryPath))
            using (var writer = new VelocityEscapedXmlTextWriter(cos))
            {
                writer.Formatting = Formatting.None; //对于 Velocity 模板，最好格式化
                xml.WriteTo(writer);
            }
        }

        private static void ProcessTableRows(XmlDocument xml, XmlNamespaceManager nsmanager)
        {
            var rowElements = FindAllRowElements(xml);

            foreach (XmlElement row in rowElements)
            {
                //检测一个行中的 table-cell 是否只包含唯一的 report-directive 元素
                //把其中的 cell 都去掉
                ProcessSingleTableRowElement(row);
            }
        }

        private static void ProcessSingleTableRowElement(XmlElement row)
        {
            var rowDirectiveElements = FindDirectiveNodesInRow(row);

            if (rowDirectiveElements.Count == 1)
            {
                row.ParentNode.ReplaceChild(rowDirectiveElements[0], row);
            }
        }

        private static List<XmlElement> FindAllRowElements(XmlDocument xml)
        {
            var nodeList = xml.GetElementsByTagName(TableRowElement);
            var rowNodes = new List<XmlElement>();

            foreach (XmlElement rowEle in nodeList)
            {
                rowNodes.Add(rowEle);
            }
            return rowNodes;
        }

        private static List<DirectiveElement> FindDirectiveNodesInRow(
            XmlElement row)
        {
            var rowDirectiveNodes = new List<DirectiveElement>(50);
            foreach (XmlElement subnode in row.ChildNodes)
            {
                var se = subnode as DirectiveElement;
                if (se != null)
                {
                    rowDirectiveNodes.Add(se);
                }
            }
            return rowDirectiveNodes;
        }

        private static void PreprocessElements(XmlDocument xml, XmlNamespaceManager nsmanager)
        {
            var placeholders = FindAllRtlElements(xml);

            foreach (XmlNode placeholder in placeholders)
            {
                string value = ExtractTemplateExpression(placeholder);

                if (value[0] == '$')
                {
                    ProcessIdentifierTag(xml, placeholder, value);

                }
                else if (value[0] == '#')
                {
                    var directiveNode = new DirectiveElement(xml, value);
                    ReduceTag(directiveNode, placeholder);
                }
                else
                {
                    throw new SyntaxErrorException();
                }
            }
        }

        private static List<XmlElement> FindAllRtlElements(XmlDocument xml)
        {
            var placeholders = new List<XmlElement>();

            var textPlaceholders = xml.GetElementsByTagName(TextPlaceholderElement);
            foreach (XmlElement tpe in textPlaceholders)
            {
                placeholders.Add(tpe);
            }

            var textAnchors = xml.GetElementsByTagName(TextAnchorElement);
            foreach (XmlElement ta in textAnchors)
            {
                var href = ta.GetAttribute("xlink:href");
                if (href != null && href.Trim().StartsWith("rtl://", StringComparison.Ordinal))
                {
                    placeholders.Add(ta);
                }
            }
            return placeholders;
        }

        private static string ExtractTemplateExpression(XmlNode placeholder)
        {
            string value = null;

            Match match = null;

            if (placeholder.Name == "text:placeholder")
            {
                match = PlaceHolderValuePattern.Match(placeholder.InnerText);
            }
            else
            {
                var href = placeholder.Attributes["xlink:href"].Value;
                match = HyperLinkValuePattern.Match(Uri.UnescapeDataString(href));
            }

            value = match.Groups[1].Value;
            CheckTemplateExpression(placeholder, value, match);

            return value;
        }

        private static void CheckTemplateExpression(XmlNode placeholder, string value, Match match)
        {

            if (match.Groups.Count != 2)
            {
                throw new SyntaxErrorException("Syntax Error: " + placeholder.InnerText);
            }

            if (value.Length < 1)
            {
                throw new SyntaxErrorException();
            }
        }

        private static void ProcessIdentifierTag(XmlDocument xml, XmlNode placeholder, string value)
        {
            Debug.Assert(xml != null);
            Debug.Assert(placeholder != null);
            Debug.Assert(!string.IsNullOrEmpty(value));

            var ie = new ReferenceElement(xml, value);

            if (placeholder.Name == TextPlaceholderElement)
            {
                ProcessPlaceHolderElement(placeholder, ie);
            }
            else
            {
                placeholder.ParentNode.ReplaceChild(ie, placeholder);
            }
        }

        private static void ProcessPlaceHolderElement(XmlNode placeholder, ReferenceElement ie)
        {
            var placeholderType = placeholder.Attributes[TextPlaceholderTypeAttribute]
                .InnerText.Trim().ToLowerInvariant(); ;
            //处理图像占位符

            switch (placeholderType)
            {
                case "image":
                    ProcessImageTag(placeholder, ie);
                    break;

                case "text":
                    placeholder.ParentNode.ReplaceChild(ie, placeholder);
                    break;

                default:
                    throw new SyntaxErrorException("Unsupported placeholder type: " + placeholderType);
            }
        }

        private static void ProcessImageTag(XmlNode placeholder, ReferenceElement ie)
        {
            Debug.Assert(placeholder != null);
            Debug.Assert(ie != null);

            //向上查找 drawbox
            var drawboxNode = LookupAncestor(placeholder, DrawTextBoxElement);
            if (drawboxNode.Name != DrawTextBoxElement)
            {
                throw new SyntaxErrorException("图像类型的占位符必须放在图文框中");
            }

            drawboxNode.ParentNode.ReplaceChild(ie, drawboxNode);
        }

        /// <summary>
        /// 化简 Tag
        /// </summary>
        /// <param name="newNode"></param>
        /// <param name="placeholder"></param>
        private static void ReduceTag(XmlNode newNode, XmlNode placeholder)
        {
            //如果上级节点只包含 placeholder 这个节点的话，那么上级也是没用的
            //以此类推，直到上级节点包含其他类型的节点或者上级节点是单元格为止

            XmlNode ancestor = placeholder;
            while (ancestor.ParentNode.ChildNodes.Count == 1)
            {
                ancestor = ancestor.ParentNode;
            }

            ancestor.ParentNode.ReplaceChild(newNode, ancestor);
        }

        /// <summary>
        /// 查找祖先元素
        /// </summary>
        /// <param name="ancestorName"></param>
        /// <param name="node"></param>
        private static XmlNode LookupAncestor(XmlNode node, string ancestorName)
        {
            XmlNode ancestor = node;
            while (ancestor.ParentNode.ChildNodes.Count == 1 &&
                ancestor.Name != ancestorName)
            {
                ancestor = ancestor.ParentNode;
            }

            return ancestor;
        }
        #endregion

    }
}
