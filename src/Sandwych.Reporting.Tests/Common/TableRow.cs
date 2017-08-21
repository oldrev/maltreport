using System;
using System.Collections.Generic;
using System.Text;

namespace Sandwych.Reporting.Tests.Common
{
    public class TableRow
    {
        public string Text { get; set; }
        public int Integer { get; set; }
        public decimal Decimal { get; set; }
        public double Double { get; set; }
        public DateTime DateTime { get; set; }
        public DateTimeOffset DateTimeOffset { get; set; }
        public TimeSpan TimeSpan { get; set; }
        public int[] Integers { get; set; }
    }
}
