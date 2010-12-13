//作者：李维
//创建时间：2010-09-03

using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;
using System.IO;
using System.Xml;

namespace Bravo.Reporting.OfficeXml
{
    public class ExcelMLDocument : SingleXmlDocumentBase
    {
        public const string IndexAttribute = "ss:Index";
        public const string ExpandedColumnCountAttribute = "ss:ExpandedColumnCount";
        public const string ExpandedRowCountAttribute = "ss:ExpandedRowCount";
        public const string FormatAttribute = "ss:Format";
        public const string TypeAttribute = "ss:Type";

        public ExcelMLDocument()
        {
        }

        public override ITemplate Compile()
        {
            return ExcelMLCompiler.Compile(this);
        }

        #region IDocument 成员

        #endregion
    }
}
