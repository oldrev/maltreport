//作者：李维
//创建时间：2010-09-03
using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.Diagnostics;
using System.Text.RegularExpressions;
using Bravo.Reporting.Xml;

namespace Bravo.Reporting.Excel2003Xml
{
    /// <summary>
    /// ODF 模板编译器
    /// 把用户创建的 ODF 文档中的 content.xml 转换为合适的 NVelocity 模板格式文件
    /// </summary>
    public class ExcelXmlTemplateCompiler : ITemplateCompiler
    {
        public const string PlaceHolderPattern =
            @"//Cell[starts-with(@ss:HRef, 'rtl://')]";

        public ITemplate Compile(IDocument doc)
        {
            if (doc.GetType() != typeof(ExcelXmlDocument))
            {
                throw new ArgumentException("只支持编译 Microsoft Excel 2003 XML 类型", "doc");
            }

            var t = new ExcelXmlTemplate();
            doc.CopyTo(t);

            var xml = t.ReadMainContentXml();
            var nsmanager = new ExcelXmlNamespaceManager(xml.NameTable);
            nsmanager.LoadOpenDocumentNamespaces();

            //第一步



            t.WriteMainContentXml(xml);

            return t;
        }
    }
}
