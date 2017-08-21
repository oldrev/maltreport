//Creation Time: 2010-08-20
using System.Diagnostics;
using System.Xml;

namespace Sandwych.Reporting.Xml
{
    /// <summary>
    /// VTL Directive XML Element
    /// </summary>
    internal sealed class DirectiveElement : XmlElement
    {
        public const string ElementName = "report-directive";

        private string _directive;

        public DirectiveElement(XmlDocument doc, string directive)
            : base(string.Empty, ElementName, string.Empty, doc)
        {
            Debug.Assert(doc != null);
            Debug.Assert(directive != null);

            this._directive = directive;
        }

        /// <summary>
        /// Write to writer
        /// </summary>
        /// <param name="w"></param>
        public override void WriteTo(XmlWriter w)
        {
            Debug.Assert(_directive != null);
            Debug.Assert(w != null);

            w.WriteRaw("{%");
            w.WriteRaw(this._directive);
            w.WriteRaw("%}");
        }

        /// <summary>
        /// 按照内容化简
        /// </summary>
        /// <param name="placeholder"></param>
        public void ReduceTagByDirective(XmlNode placeholder)
        {
            //如果上级节点的 InnerText 只包含 placeholder 这个节点的话，那么上级也是没用的
            //以此类推，直到上级节点包含其他类型的节点
            string placeholderText = placeholder.InnerText;
            XmlNode ancestor = placeholder;
            while (ancestor.ParentNode.InnerText == placeholderText)
            {
                ancestor = ancestor.ParentNode;
            }

            ancestor.ParentNode.ReplaceChild(this, ancestor);
        }

        /// <summary>
        /// 按照数量化简 Tag
        /// </summary>
        /// <param name="placeholder"></param>
        public void ReduceTagByCount(XmlNode placeholder)
        {
            //如果上级节点只包含 placeholder 这个节点的话，那么上级也是没用的
            //以此类推，直到上级节点包含其他类型的节点

            XmlNode ancestor = placeholder;
            while (ancestor.ParentNode.ChildNodes.Count == 1)
            {
                ancestor = ancestor.ParentNode;
            }

            ancestor.ParentNode.ReplaceChild(this, ancestor);
        }
    }
}