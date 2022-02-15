using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Sandwych.Reporting;
using Sandwych.Reporting.OpenDocument;
using MaltReport.HelloWorld;

// Prepare your data: 
var desktopDir = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
var outputFile = Path.Combine(desktopDir, "generated.odt");

var employees = new Employee[]
{
    new Employee{ Name = "Micheal Scott", JobTitle = "Branch Manager" },
    new Employee{ Name = "Pam Beesly", JobTitle = "Office Administrator" },
    new Employee{ Name = "Jim Halpert", JobTitle = "Salesman" },
    new Employee{ Name = "Dwight Schrute", JobTitle = "Salesman" },
    new Employee{ Name = "Andy Bernard", JobTitle = "Salesman" },
};

var myImage = new ImageBlob("jpeg", await File.ReadAllBytesAsync("Image.jpeg"));

var data = new Dictionary<string, object>()
{
    { "employees", employees },
    { "myImage", myImage },
};

var context = new TemplateContext(data);

// Load document template:
var templateDocument = await OdfDocument.LoadFromAsync("EmployeesTemplate.odt");
var template = new OdtTemplate(templateDocument);

// Rendering step
var result = await template.RenderAsync(context);

// Got result:
await result.SaveAsync(outputFile);
Console.WriteLine("All done, checkout the generated document: '{0}'", outputFile);

Console.WriteLine("Press any key to exit.");
Console.ReadKey();

return 0;
