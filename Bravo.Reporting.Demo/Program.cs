using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Xml;
using System.Data;
using System.Data.SQLite;

using Bravo.Reporting.OpenDocument;
using Bravo.Reporting.OfficeXml;

namespace Bravo.Reporting.Demo
{
    class Program
    {
        static void Main(string[] args)
        {
            var dt = new DataTable("Employees");

            //从数据库里查询数据填充 DataTable
            var connectionString = @"Data Source=Database/northwind.db;Version=3;";
            using (var connection = new SQLiteConnection(connectionString))
            {
                var adapter = new SQLiteDataAdapter();
                adapter.SelectCommand = new SQLiteCommand(
                    "SELECT * FROM Employees", connection);
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
                {"orm_employees",
                    new List<Employee>()
                    {
                        new Employee("张三", "张三的地址", 22),
                        new Employee("李四", "李四的地址", 27),
                        new Employee("牛二", "牛二的地址", 33),
                        new Employee("小明", "小明的地址", 19),
                    }
                },

                {"employees", dt}, //可以传递 DataTable 给模板

                {"now", DateTime.Now}, //任何类型都可以传给模板
            };

            //Bravo.Reporting 文档生成三部曲：
            var odt = new OdfDocument();
            //1. 加载用户创建的模板文档
            odt.Load("template1.odt");
            //2. 编译模板，编译结果 template1 可以保存到磁盘上避免多次编译
            var template1 = odt.Compile();
            //3. 按照用户提供的上下文数据渲染模板得到结果
            var result1 = template1.Render(renderContext);
            using (var resultFile = File.Open("result1.odt", FileMode.Create, FileAccess.ReadWrite))
            {
                result1.Save(resultFile);
            }

            RenderTemplate<OdfDocument>(renderContext, "template2.ods", "result2.ods");
            RenderTemplate<ExcelMLDocument>(renderContext, "template3.xls", "result3.xls.xml");
            RenderTemplate<WordMLDocument>(renderContext, "template4.doc", "result4.doc.xml");

            //编译报表用于把用户设计的原始报表文档转换为可以用于直接渲染的模板
            //编译的结果可以缓存在内存中也可以保存在文件系统中多次使用
            //模板编译后的结果可以保存在磁盘上用于多次渲染

            Console.WriteLine("成功完成模板渲染");

            Console.ReadKey();
        }

        private static void RenderTemplate<T>(
            IDictionary<string, object> ctx, 
            string templateFileName,
            string resultFileName)
            where T : IDocument, new()
        {
            Console.WriteLine("正在生成模板 '{0}' ...", resultFileName);
            var doc = new T();
            doc.Load(templateFileName);
            var t = doc.Compile();
            var result3 = t.Render(ctx);
            using (var resultFile3 = File.Open(
                resultFileName, FileMode.Create, FileAccess.ReadWrite))
            {
                result3.Save(resultFile3);
            }
        }
    }
}
