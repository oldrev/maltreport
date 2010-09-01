using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Xml;
using System.Data;
using System.Data.SqlClient;

using Bravo.Reporting.OpenDocument;

namespace Bravo.Reporting.Demo
{
    class Program
    {
        static void Main(string[] args)
        {
            var dt = new DataTable("T_BIZ_CATEGORY");

            //从数据库里查询数据填充 DataTable
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

            var renderContext = new Dictionary<string, object>()
            {
                //传递简单的类型
                {"title", "公司雇员一览表"},
                {"property1", "属性1"},
                {"property2", "属性2"},

                // 可以传递具有强类型的对象数据给模板
                {"employees",
                    new List<Employee>()
                    {
                        new Employee("张三", "张三的地址", 22),
                        new Employee("李四", "李四的地址", 27),
                        new Employee("牛二", "牛二的地址", 33),
                        new Employee("小明", "小明的地址", 19),
                    }
                },

                {"categories", dt}, //可以传递 DataTable 给模板

                {"now", DateTime.Now}, //任何类型都可以传给模板
            };


            var odt = new OdfDocument("template1.odt"); //加载模板文档
            var ods = new OdfDocument("template2.ods");

            //编译报表，把用户设计的原始报表转换为可以用于直接渲染的模板
            //编译的结果可以缓存在内存中也可以保存在文件系统中多次使用
            var compiler = new OdfTemplateCompiler();
            var template1 = compiler.Compile(odt);
            var template2 = compiler.Compile(ods);

            //保存编译后的报表，这并不是必须的
            using (var stFile = File.Open("template1.odt.st", FileMode.Create, FileAccess.ReadWrite))
            {
                template1.Save(stFile);
            }

            using (var stFile = File.Open("template2.ods.st", FileMode.Create, FileAccess.Write))
            {
                template2.Save(stFile);
            }

            var tr1 = new OdfTemplateRenderer(template1);
            var result1 = tr1.Render(renderContext);

            Console.WriteLine("正在生成 ODT 模板...");
            using (var resultFile = File.Open(
                "result1.odt", FileMode.Create, FileAccess.ReadWrite))
            {
                result1.Save(resultFile);
            }

            Console.WriteLine("正在生成 ODS 模板...");
            var tr2 = new OdfTemplateRenderer(template2);
            var result2 = tr2.Render(renderContext);
            using (var resultFile2 = File.Open(
                "result2.ods", FileMode.Create, FileAccess.ReadWrite))
            {
                result2.Save(resultFile2);
            }

            Console.WriteLine("成功完成模板渲染");

            Console.ReadKey();
        }
    }
}
