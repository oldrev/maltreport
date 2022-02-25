using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Sandwych.Reporting;
using Sandwych.Reporting.Odf;
using MaltReport.HelloWorld;

// Prepare your data: 
var desktopDir = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
var outputFile = Path.Combine(desktopDir, "generated.odt");

var model = new
{
    employees = new Employee[] 
    {
        new Employee { Name = "Micheal Scott",  JobTitle = "Regional Manager" },
        new Employee { Name = "Pam Beesly",     JobTitle = "Office Administrator" },
        new Employee { Name = "Jim Halpert",    JobTitle = "Salesman" },
        new Employee { Name = "Dwight Schrute", JobTitle = "Assistant to the Regional Manager" },
        new Employee { Name = "Andy Bernard",   JobTitle = "Salesman" },
    },
    myImage = await Blob.LoadAsync("Image.jpeg"),
};

var context = new TemplateContext(model);
context.AllowMembersAccessTo(typeof(Employee));

// Load document template:
var templateDocument = await OdfDocument.LoadFromAsync("EmployeesTemplate.odt");
var template = await OdfTemplateCompiler.Instance.CompileAsync(templateDocument);

// Rendering step
var result = await template.RenderAsync(context);

// Got result:
await result.SaveAsync(outputFile);
Console.WriteLine("All done, checkout the generated document: '{0}'", outputFile);

Console.WriteLine("Press any key to exit.");
Console.ReadKey();

return 0;
