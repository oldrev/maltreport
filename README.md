# MaltReport

[![NuGet Stats](https://img.shields.io/nuget/v/MaltReport2.svg)](https://www.nuget.org/packages/MaltReport2) 
[![Build status](https://ci.appveyor.com/api/projects/status/w9rc2jbb6v4o4jgk/branch/master?svg=true)](https://ci.appveyor.com/project/oldrev/maltreport/branch/master)

MaltReport is an OpenDocument/OfficeOpenXML powered reporting engine for .NET and Mono, it is inspired by the excellent [Relatorio Project](http://relatorio.openhex.org/).

There is a brief introduction in Chinese: [MaltReport2 中文使用简介](http://www.cnblogs.com/oldrev/p/maltreport2_intro.html)

MaltReport DO NOT requires MS-Office or LibreOffice to render report, instead MaltReport manipulates ODT/ODS/XLS/DOC file by itself, so it fast & reliable. 

The template of your report is just a normal document created with LibreOffice or MS-Word/MS-Excel (ODT/ODS and XLS/DOC are fully supported), 
we can use LibreOffice and MS-Word/Excel as WYSIWYG report development tools.

I do not have too much time working with the document, so to learn the usage of MaltReport please see the Sandwych.Reporting.Demo project.


## Getting Started

### Prerequisites

* Visual Studio 2015: This project is written in C# 5.0 using Microsoft Visual Studio 2015 Community Edition.

### Supported Platform

* .NET Framework 4.5+
* The support of .NET Core is working in progress.

### Installation

MaltReport can be installed from [NuGet](https://www.nuget.org/packages/MaltReport2) or type following commands in the NuGet Console:

```
PM> Install-Package MaltReport2
```

## Demo & Usage:

### Step 1: Organize your Data into a IDictionary<string,object>

```csharp

public class Employee {
    public Employee(string name, string address) {
        this.Name = name;
        this.Address = address;
    }

    public string Name {get; private set; }
    public string Address {get; private set; }

}

var renderContext = new Dictionary<string, object>() {
    //Plain old types
    {"title", "EMPLOYEES SHEET"},
    {"property1", "Property Value 1"},
    {"property2", "Property Value 2"},

    //Strong-type objects
    {"orm_employees",
        new List<Employee>()
        {
            new Employee("Micheal Scott", "Address 1"),
            new Employee("Andy Bernard", "Address 3"),
            new Employee("Dwight Shurte", "Address 1"),
            new Employee("Jim Halpert", "Address 2"),
            new Employee("Pam Beesly", "Address 4"),
        }
    },
    {"now", DateTime.Now}, 
};
```

### Step 2: Render your template

```csharp
var template = new WordMLTemplate();
template.Load("template1.xml"); //Load template file
template.Compile(); //Compile template
var resultDoc = template.Render(ctx); //Render template with data

//Save output to a DOC file
using (var resultFile3 = File.Open("c:\\tmp\\result.doc", FileMode.Create, FileAccess.ReadWrite)) { 
    resultDoc.Save(resultFile3);
}
```

## License

MaltReport is licensed in the MIT license.

Author: Wei "oldrev" Li <oldrev<at>gmail.com>

* Copyright (C) 2009 Wei "oldrev" Li
* Copyright (C) 2010-2016 Sandwych Consulting LLC.
* Copyright (C) 2017-TODAY Binary Stars Technologies LLC.
