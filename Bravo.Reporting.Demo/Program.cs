using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace Bravo.Reporting.Demo
{
    class Program
    {
        static void Main(string[] args)
        {
            var odf = new OdfArchive("template1.odt");

            var compiler = new OdtTemplateCompiler();
            var template = compiler.Compile(odf);

            var data = new Dictionary<string, object>()
            {
                {"property1", "属性1"},
                {"property2", "属性2"},
                {"items",
                    new string[]
                    {
                        "张三", "李四", "牛二"
                    }
                },
            };

            var result = TemplateProcessor.Process(template, data);

            result.OdfPath = "result1.odt";

            using (var resultFile = File.Open("result1.odt", FileMode.CreateNew, FileAccess.Write))
            {
                result.Save(resultFile);
            }

            Console.ReadLine();
        }
    }
}
