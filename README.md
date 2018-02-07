# MaltReport

[![NuGet Stats](https://img.shields.io/nuget/v/MaltReport2.svg)](https://www.nuget.org/packages/MaltReport2) 
[![Build status](https://ci.appveyor.com/api/projects/status/7kj4cnl64negfdn6/branch/master?svg=true)](https://ci.appveyor.com/project/oldrev/maltreport/branch/master)
[![Build Status](https://travis-ci.org/oldrev/maltreport.svg?branch=master)](https://travis-ci.org/oldrev/maltreport)


MaltReport is an OpenDocument/OfficeOpenXML powered reporting engine for .NET and Mono, 
it is inspired by the excellent [Relatorio Project](http://relatorio.openhex.org/).

There is a briefly introduction in Chinese: [MaltReport2 中文使用简介](http://www.cnblogs.com/oldrev/p/maltreport2_intro.html)

MaltReport DO NOT REQUIRES MS-Office or LibreOffice to generate document. 
Instead MaltReport manipulates ODT/ODS/XLS/DOC file by itself, so it could be fast & reliable.

## Documents

* [Wiki](https://github.com/oldrev/maltreport/wiki)

## Getting Started

### Prerequisites

* Visual Studio 2017: This project is written in C# 7.0 using Microsoft Visual Studio 2017 Community Edition.

### Supported Platform

* .NET Framework 4.6.1
* .NET Standard 1.6+

### Installation

MaltReport can be installed from [NuGet](https://www.nuget.org/packages/MaltReport2).

## Demo & Usage:

### Step 1: Organize your data into Context

```csharp
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
```

### Step 2: Prepare your template

![Template](https://github.com/oldrev/maltreport/raw/dev/screenshots/hello-world/template.png)

### Step 3: Load & render your template

```csharp

using (var stream = File.OpenRead("EmployeesTemplate.odt"))
{
    var odt = OdfDocument.LoadFrom(stream);
    var template = new OdtTemplate(odt);

    var result = template.Render(context);

    var desktopDir = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
    var outputFile = Path.Combine(desktopDir, "generated.odt");

    result.Save(outputFile);
}
```

### Step 4: Check the generated document out:

![Generated Document](https://github.com/oldrev/maltreport/raw/dev/screenshots/hello-world/generated.png)


## License

MaltReport is licensed in the MIT license.

Author: Wei "oldrev" Li <oldrev@gmail.com>

* Copyright (C) 2009 Wei "oldrev" Li
* Copyright (C) 2010-2016 Sandwych Consulting LLC.
* Copyright (C) 2017-TODAY Binary Stars Technologies LLC. & Contributors

## CREDITS

* [Fluid](https://github.com/sebastienros/fluid)
