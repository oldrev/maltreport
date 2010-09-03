using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Xml;
using System.Data;
using System.Data.SqlClient;

using Bravo.Reporting.OpenDocument;
using Bravo.Reporting.Excel2003Xml;

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


            var odt = new OdfDocument(); //加载模板文档
            odt.Load("template1.odt");
            var ods = new OdfDocument();
            ods.Load("template2.ods");
            var xls = new ExcelXmlDocument();
            xls.Load("template3.xls");

            //编译报表，把用户设计的原始报表转换为可以用于直接渲染的模板
            //编译的结果可以缓存在内存中也可以保存在文件系统中多次使用
            var compiler = new OdfTemplateCompiler();
            var template1 = compiler.Compile(odt);
            var template2 = compiler.Compile(ods);

            var xlsCompiler = new ExcelXmlTemplateCompiler();
            var template3 = xlsCompiler.Compile(xls);

            //模板编译后的结果可以保存在磁盘上用于多次渲染

            var result1 = template1.Render(renderContext);

            Console.WriteLine("正在生成 ODT 模板...");
            using (var resultFile = File.Open(
                "result1.odt", FileMode.Create, FileAccess.ReadWrite))
            {
                result1.Save(resultFile);
            }

            Console.WriteLine("正在生成 ODS 模板...");
            var result2 = template2.Render(renderContext);
            using (var resultFile2 = File.Open(
                "result2.ods", FileMode.Create, FileAccess.ReadWrite))
            {
                result2.Save(resultFile2);
            }

            Console.WriteLine("正在生成 Excel 2003 XML 格式模板...");
            var result3 = template3.Render(renderContext);
            using (var resultFile3 = File.Open(
                "result3.xls", FileMode.Create, FileAccess.ReadWrite))
            {
                result3.Save(resultFile3);
            }

            Console.WriteLine("成功完成模板渲染");

            Console.ReadKey();
        }
    }
}
