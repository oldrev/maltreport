using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Xml;
using System.Data;
using Community.CsharpSqlite.SQLiteClient;

using Malt.Reporting.OpenDocument;
using Malt.Reporting.OfficeXml;

namespace Malt.Reporting.Demo
{
    class Program
    {
        static void Main(string[] args)
        {
            var dt = new DataTable("Employees");

            //Fill the DataTable
            var connectionString = @"Version=3,uri=file://./Database/northwind.db";
            using (var connection = new SqliteConnection(connectionString))
            {
                var sql = "SELECT FirstName, LastName, HireDate, BirthDate, Address FROM Employees";
                var adapter = new SqliteDataAdapter();
                adapter.SelectCommand = new SqliteCommand(sql, connection);
                adapter.FillSchema(dt, SchemaType.Source);
                adapter.Fill(dt);
            }

            var renderContext = new Dictionary<string, object>()
            {
                //Plain old types
                {"title", "EMPLOYEES"},
                {"property1", "Property 1"},
                {"property2", "Property 2"},

                //Strong types
                {"orm_employees",
                    new List<Employee>()
                    {
                        new Employee("Micheal Scott", "Address 1", 22),
                        new Employee("Andy Bernard", "Address 3", 33),
                        new Employee("Dwight Shurte", "Address 1", 22),
                        new Employee("Jim Halpert", "Address 2", 27),
                        new Employee("Pam Beesly", "Address 4", 19),
                    }
                },

                {"employees", dt}, //DataTable is ok

                {"now", DateTime.Now}, //DateTime is ok too
            };

            RenderTemplate<OdfTemplate>(renderContext, "template1.odt", "result1.odt");
            RenderTemplate<OdfTemplate>(renderContext, "template2.ods", "result2.ods");
            RenderTemplate<ExcelMLTemplate>(renderContext, "template3.xls", "result3.xls.xml");
            RenderTemplate<WordMLTemplate>(renderContext, "template4.doc", "result4.doc.xml");

            Console.WriteLine("All done. Press any key to exit...");

            Console.ReadKey();
        }

        private static void RenderTemplate<T>(
            IDictionary<string, object> ctx,
            string templateFileName,
            string resultFileName)
            where T : ITemplate, new()
        {
            Console.WriteLine("Generating '{0}' ...", resultFileName);
            //3 steps to render a template:
            var template = new T();
            template.Load(templateFileName); //Step 1: load the template file
            template.Compile(); //Step 2: compile the template
            var resultDoc = template.Render(ctx); //Step 3: Render template with user data
            using (var resultFile3 = File.Open(
                resultFileName, FileMode.Create, FileAccess.ReadWrite))
            {
                //Finally, save it into a file
                resultDoc.Save(resultFile3);
            }
        }
    }
}
