using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

using org.artofsolving.jodconverter;
using org.artofsolving.jodconverter.office;


namespace Sandwych.Reporting.JODConverterDemo
{
    class Program
    {
        static void Main(string[] args)
        {
            //Make sure your LibreOffice/OpenOffice home is correct
            var officeHome = @"C:\LibreOffice3.5";

            var officePaths =
                Path.Combine(officeHome, "URE", "bin") + ";" + Path.Combine(officeHome, "program");
            var path = Environment.GetEnvironmentVariable("PATH");
            path += ";" + officePaths;
            Environment.SetEnvironmentVariable("PATH", path);

            var officeManager = new DefaultOfficeManagerConfiguration()
                .setOfficeHome(officeHome)
                .buildOfficeManager();

            officeManager.start();
            var converter = new OfficeDocumentConverter(officeManager);


            //Do some document converting job
            converter.convert(new java.io.File("demo.odt"), new java.io.File("test.pdf"));


            officeManager.stop();
            Console.ReadKey();
        }
    }
}
