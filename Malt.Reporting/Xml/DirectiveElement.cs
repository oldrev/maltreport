//Creation Time: 2010-08-20
using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.Text.RegularExpressions;
using System.Diagnostics;

namespace Malt.Reporting.Xml
{
    /// <summary>
    /// VTL Directive XML Element
    /// </summary>
    internal sealed class DirectiveElement : XmlElement
    {
        public const string ElementName = "report-directive";
        private string directive;

        public DirectiveElement(XmlDocument doc, string directive)
            : base(string.Empty, ElementName, string.Empty, doc)
        {
            Debug.Assert(doc != null);
            Debug.Assert(directive != null);

            //WORKAROUND:
            //由于 NVelocity 1.1 不支持 #{end} 形式的 Directive，因此遇到类似 #endWHATEVER_STRING
            //的 Directive 会造成解析错误，这里我们在后边加上一个 VTL 注释隔开 #end#**#WHATEVER_STRING 

            if (directive[directive.Length - 1] == ')')
            {
                this.directive = directive;
            }
            else
            {
                this.directive = directive + "#**#";
            }
        }

        /// <summary>
        /// Write to writer
        /// </summary>
        /// <param name="w"></param>
        public override void WriteTo(XmlWriter w)
        {
            Debug.Assert(directive != null);
            Debug.Assert(w != null);

            w.WriteRaw(this.directive);
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
