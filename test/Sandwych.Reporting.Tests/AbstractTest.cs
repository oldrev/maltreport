using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Reflection;
using NUnit.Framework;

namespace Sandwych.Reporting.Tests
{
    public abstract class AbstractTest
    {
        public static string TempDirPath { get; } = Path.Combine(Path.GetTempPath(), "MaltReportTest");

        [SetUp]
        protected void SetUp() {
            if(!Directory.Exists(TempDirPath)) {
                Directory.CreateDirectory(TempDirPath);
            }
        }

        public static Stream GetResource(string path)
        {
            var fullPath = "Sandwych.Reporting.Tests.Resources." + path;
            return typeof(AbstractTest).GetTypeInfo().Assembly.GetManifestResourceStream(fullPath);
        }

        public static Stream GetTemplate(string path) =>
            GetResource("Templates." + path);




    }

}
