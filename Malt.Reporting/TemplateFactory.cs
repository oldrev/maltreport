using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace Malt.Reporting
{
    internal static class TemplateFactory
    {
        //硬编码，但是简单 :)，没必要引入 MEF 等
        private readonly static IDictionary<string, Func<ITemplate>> ExtensionCreatorMapping =
            new Dictionary<string, Func<ITemplate>>()
            {
                { "xls", () => new OfficeXml.ExcelMLTemplate() },
                { "doc", () => new OfficeXml.WordMLTemplate() },
                { "odt", () => new OpenDocument.OdfTemplate() },
                { "ods", () => new OpenDocument.OdfTemplate() },
            };

        public static ITemplate CreateTemplateByFileName(string path)
        {
            var extensionName = Path.GetExtension(path);
            extensionName = extensionName.ToLowerInvariant();
            Func<ITemplate> proc = null;
            if (ExtensionCreatorMapping.TryGetValue(extensionName, out proc))
            {
                return proc();
            }
            else
            {
                throw new NotSupportedException("Unsupported extension name: " + extensionName);
            }
        }

    }
}
