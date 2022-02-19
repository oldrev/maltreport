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
        public string TempPath { get; private set; }

        [SetUp]
        protected void SetUp() {
            this.TempPath = Path.Combine(Path.GetTempPath(), "MaltReportTest");
            if(!Directory.Exists(this.TempPath)) {
                Directory.CreateDirectory(this.TempPath);
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
