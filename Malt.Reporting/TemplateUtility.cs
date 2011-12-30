using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Diagnostics;

using Malt.Reporting.OfficeXml;
using Malt.Reporting.OpenDocument;

namespace Malt.Reporting
{
    public static class TemplateUtility
    {
        private static void RenderTemplateFile<DocType, TempType>(
                IDictionary<string, object> ctx,
                string templateFileName, string resultFileName)
            where DocType : IDocument, new()
            where TempType : ITemplate, new()
        {
            var template = new TempType();
            using (var ts = File.OpenRead(templateFileName))
            {
                template.Load(ts);
                template.Compile();
                var resultDoc = template.Render(ctx);
                using (var resultFile3 = File.Open(
                    resultFileName, FileMode.Create, FileAccess.ReadWrite))
                {
                    resultDoc.Save(resultFile3);
                }
            }
        }

        public static void RenderTemplateFile(
            IDictionary<string, object> ctx, string templatePath, Stream outStream)
        {
            var template = TemplateFactory.CreateTemplateByFileName(templatePath);
            template.Load(templatePath);
            template.Compile();
            var resultDoc = template.Render(ctx);
            resultDoc.Save(outStream);
        }

        public static void RenderTemplateFile(
            IDictionary<string, object> ctx, string templatePath, string resultPath)
        {
            using (var fs = File.OpenWrite(resultPath))
            {
                RenderTemplateFile(ctx, templatePath, fs);
            }
        }

    }
}
