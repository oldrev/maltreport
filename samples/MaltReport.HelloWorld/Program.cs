using System;
using System.Collections.Generic;
using System.IO;
using Sandwych.Reporting;
using Sandwych.Reporting.OpenDocument;

namespace MaltReport.HelloWorld
{
    class Program
    {
        static void Main(string[] args)
        {
            var employees = new Employee[]
            {
                new Employee{ Name = "Micheal Scott", JobTitle = "Branch Manager" },
                new Employee{ Name = "Pam Beesly", JobTitle = "Office Administrator" },
                new Employee{ Name = "Jim Halpert", JobTitle = "Salesman" },
                new Employee{ Name = "Dwight Schrute", JobTitle = "Salesman" },
                new Employee{ Name = "Andy Bernard", JobTitle = "Salesman" },
            };

            var image = new ImageBlob("jpeg", File.ReadAllBytes("Image.jpeg"));

            var data = new Dictionary<string, object>()
            {
                { "employees", employees },
                { "image", image },
            };

            var context = new TemplateContext(data);

            using (var stream = File.OpenRead("EmployeesTemplate.odt"))
            {
                var odt = OdfDocument.LoadFrom(stream);
                var template = new OdtTemplate(odt);


                var result = template.Render(context);

                var desktopDir = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
                var outputFile = Path.Combine(desktopDir, "generated.odt");

                result.Save(outputFile);
                Console.WriteLine("All done, checkout the generated document: {0}", outputFile);
            }

            Console.WriteLine("Press any key to exit.");
            Console.ReadKey();
        }
    }
}
