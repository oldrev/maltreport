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

    }

}
