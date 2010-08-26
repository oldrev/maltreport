using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Xml;
using System.Data;
using System.Data.SqlClient;

namespace Bravo.Reporting.Demo
{
    class Program
    {
        static void Main(string[] args)
        {
            var dt = new DataTable("T_BIZ_CATEGORY");

            var connectionString =
                @"Data Source=.\SQLEXPRESS;Initial Catalog=VoucherDev;Integrated Security=True;";
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                SqlDataAdapter adapter = new SqlDataAdapter();
                adapter.SelectCommand = new SqlCommand(
                    "SELECT * FROM T_BIZ_CATEGORY", connection);
                adapter.FillSchema(dt, SchemaType.Source);
                adapter.Fill(dt);
            }

            var context = new Dictionary<string, object>()
            {
                {"title", "公司雇员一览表"},
                {"property1", "属性1"},
                {"property2", "属性2"},
                {"employees",
                    new List<Employee>()
                    {
                        new Employee("张三", "张三的地址", 22),
                        new Employee("李四", "李四的地址", 27),
                        new Employee("牛二", "牛二的地址", 33),
                        new Employee("小明", "小明的地址", 19),
                    }
                },
                {"categories", dt},
                {"now", DateTime.Now},
            };

            try
            {

                var odt = new OdfDocument("template1.odt");
                var ods = new OdfDocument("template2.ods");

                //编译报表，把用户设计的原始报表转换为可以用于直接渲染的模板
                //编译的结果可以缓存在内存中也可以保存在文件系统中多次使用
                var compiler = new OdfTemplateCompiler();
                var template1 = compiler.Compile(odt);
                var template2 = compiler.Compile(ods);

                using (var stFile = File.Open("template1.odt.st", FileMode.Create, FileAccess.ReadWrite))
                {
                    template1.Save(stFile);
                }

                var result1 = TemplateRenderer.Render(template1, context);
                var result2 = TemplateRenderer.Render(template2, context);

                Console.WriteLine("正在生成 ODT 模板...");
                using (var resultFile = File.Open(
                    "result1.odt", FileMode.Create, FileAccess.ReadWrite))
                {
                    result1.Save(resultFile);
                }

                Console.WriteLine("正在生成 ODS 模板...");
                using (var resultFile2 = File.Open(
                    "result2.ods", FileMode.Create, FileAccess.ReadWrite))
                {
                    result2.Save(resultFile2);
                }

                Console.WriteLine("成功完成模板渲染");

            }
            catch (Exception ex)
            {
                Console.WriteLine("错误：" + ex.Message);
            }

            Console.ReadKey();
        }
    }
}
