using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.IO;

namespace Bravo.Reporting
{

    /// <summary>
    /// 内部使用的 ST 模板写入器
    /// </summary>
    internal class StringTemplateWriter : XmlTextWriter
    {
        public enum TagType
        {
            ForEach
        }

        private Stack<TagType> stTags = new Stack<TagType>();

        public StringTemplateWriter(Stream w, Encoding encoding)
            : base(w, encoding)
        {
        }

        public StringTemplateWriter(TextWriter w)
            : base(w)
        {
        }

        public void WriteStartForEachTag(string expression)
        {
            this.stTags.Push(TagType.ForEach);
            this.WriteRaw("$" + expression + "{ ");
        }

        public void WriteEndForEachTag(string expression)
        {
            var tag = this.stTags.Pop();
            if (tag != TagType.ForEach)
            {
                throw new SyntaxErrorException();
            }
            this.WriteRaw("}$");
        }

        public void WritePlaceHolder(string expression)
        {
            this.WriteRaw("$" + expression + "$");
        }
    }
}
