using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

using Bravo.Reporting.Test;

namespace Bravo.Reporting.Office2003Xml.Test
{
    internal static class ExcelMLBehaviors
    {
        static readonly string[] ExcelML2003SchemaFiles = new string[] 
        {
            @"resources/schemas/excel2003/c.xsd",
            @"resources/schemas/excel2003/dt.xsd",
            @"resources/schemas/excel2003/excel.xsd",
            @"resources/schemas/excel2003/excel2003xml.xsd",
            @"resources/schemas/excel2003/excelss.xsd",
            @"resources/schemas/excel2003/office.xsd",
            @"resources/schemas/excel2003/rowsetschema.xsd",
            @"resources/schemas/excel2003/rowsset.xsd",
            @"resources/schemas/excel2003/schema.xsd",
            @"resources/schemas/excel2003/udc.xsd",
            @"resources/schemas/excel2003/udcsoap.xsd",
            @"resources/schemas/excel2003/udcxmlfile.xsd",
            @"resources/schemas/excel2003/vml.xsd",
        };

        /// <summary>
        /// 通过微软提供的 Excel 2003 XML 架构文件来检查生成的
        /// Excel 文档结构的有效性
        /// </summary>
        /// <param name="xml"></param>
        public static void ShouldBeWellFormedExcelML(this XmlDocument xml)
        {
            xml.ShouldWellFormed(ExcelML2003SchemaFiles);
        }
    }
}
