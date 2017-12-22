using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Reflection;


namespace Sandwych.Reporting.Tests
{
    public abstract class AbstractTest
    {
        public string TempPath { get; }

        public AbstractTest() {
            this.TempPath = Path.Combine(Path.GetTempPath(), "MaltReportTest");
            if(!Directory.Exists(this.TempPath)) {
                Directory.CreateDirectory(this.TempPath);
            }
        }

    }

}
