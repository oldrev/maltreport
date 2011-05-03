using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Diagnostics;

using Bravo.Reporting.OfficeXml;
using Bravo.Reporting.OpenDocument;

namespace Bravo.Reporting
{
    public static class TemplateUtility
    {
        private static void RenderTemplateFile<DocType, TempType>(
                IDictionary<string, object> ctx,
                string templateFileName, string resultFileName)
            where DocType : IDocument, new()
            where TempType : IDocument
        {
            var doc = new DocType();
            using (var ts = File.OpenRead(templateFileName))
            {
                doc.Load(ts);
                var t = doc.Compile();
                var result3 = t.Render(ctx);
                using (var resultFile3 = File.Open(
                    resultFileName, FileMode.Create, FileAccess.ReadWrite))
                {
                    result3.Save(resultFile3);
                }
            }
        }

        public static void RenderOdfTemplate(
                IDictionary<string, object> ctx,
                string templateFileName, string resultFileName)
        {
            RenderTemplateFile<OdfDocument, OdfDocument>(
                ctx, templateFileName, resultFileName);
        }

        public static void RenderExcelTemplate(
                IDictionary<string, object> ctx,
                string templateFileName, string resultFileName)
        {
            RenderTemplateFile<ExcelMLDocument, ExcelMLDocument>(
                ctx, templateFileName, resultFileName);
        }
    }
}
