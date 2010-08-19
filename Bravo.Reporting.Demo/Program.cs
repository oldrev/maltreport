using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Xml;

namespace Bravo.Reporting.Demo
{
    class Program
    {
        static void Main(string[] args)
        {
            var data = new Dictionary<string, object>()
            {
                {"property1", "属性1 > > > < ;;;"},
                {"property2", "属性2"},
                {"items",
                    new string[]
                    {
                        "张三", "李四", "牛二"
                    }
                },
            };


            try
            {

                var odf = new OdfArchive("template1.odt");

                var compiler = new OdtTemplateCompiler();
                var template = compiler.Compile(odf);



                var result = TemplateRenderer.Render(template, data);

                result.OdfPath = "result1.odt";

                using (var resultFile = File.Open("result1.odt", FileMode.CreateNew, FileAccess.Write))
                {
                    result.Save(resultFile);
                }

                Console.WriteLine("成功完成模板渲染");

            }
            catch (Exception ex)
            {
                Console.WriteLine("错误：" + ex.Message);
            }

            Console.ReadLine();
        }
    }
}
