# MaltReport

[![Build](https://github.com/oldrev/maltreport/actions/workflows/build-and-test.yml/badge.svg)](https://github.com/oldrev/maltreport/actions/workflows/build-and-test.yml)
[![NuGet Stats](https://img.shields.io/nuget/v/MaltReport2.svg)](https://www.nuget.org/packages/MaltReport2) 

**PLEASE NOTE**: 
* At this very moment I'm rewriting the old library, the next coming v3.0 will be a incompatible version;
* I'm also working on a additional commerce library for the docx/xlsx support, closed-source library for MS-Office seems fair;
* The super old and buggy OfficeML support will be dropped, the OfficeML format is a legacy from 2003;
* More real world unit tests, more documents;
* Check `dev` branch to track the progress of the incoming v3 series.

MaltReport is an OpenDocument/OfficeOpenXML powered document template engine for .NET and Mono, 
it is inspired by the excellent [Relatorio Project](http://relatorio.openhex.org/).

There is a briefly introduction in Chinese: [MaltReport2 中文使用简介](http://www.cnblogs.com/oldrev/p/maltreport2_intro.html)

MaltReport DO NOT REQUIRES MS-Office or LibreOffice to generate document. 
Instead MaltReport manipulates ODT/ODS/XLS/DOC file by itself, so it could be fast & reliable.

## Documents

* [Wiki](https://github.com/oldrev/maltreport/wiki)

## Getting Started

### Prerequisites

* Visual Studio 2022: This project is written in C# 9.0 using Microsoft Visual Studio 2022 Community Edition.

### Supported Platform

* .NET 5.0/6.0
* .NET Framework 4.6.1+
* .NET Standard 2.0+

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

var templateDocument = await OdfDocument.LoadFromAsync("EmployeesTemplate.odt");
var template = new OdtTemplate(templateDocument);

var result = template.Render(context);

var desktopDir = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
var outputFile = Path.Combine(desktopDir, "generated.odt");

result.Save(outputFile);
```

### Step 4: Check the generated document out:

![Generated Document](https://github.com/oldrev/maltreport/raw/dev/screenshots/hello-world/generated.png)


## License

MaltReport is licensed in the MIT license.

Author: Wei "oldrev" Li <oldrev@gmail.com>

See `LICENSE.txt` for details.

## CREDITS

* [Fluid Template Engine](https://github.com/sebastienros/fluid)



[linux-build-badge]: https://github.com/oldrev/maltreport/workflows/Linux%20Build/badge.svg
[linux-build]: https://github.com/oldrev/maltreport/actions?query=workflow%3A%22Linux+Build%22
[macos-build-badge]: https://github.com/oldrev/maltreport/workflows/MacOS%20Build/badge.svg
[macos-build]: https://github.com/oldrev/maltreport/actions?query=workflow%3A%22MacOS+Build%22
[windows-build-badge]: https://github.com/oldrev/maltreport/workflows/Windows%20Build/badge.svg
[windows-build]: https://github.com/oldrev/maltreport/actions?query=workflow%3A%22Windows+Build%22
