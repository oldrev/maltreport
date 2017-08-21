using System;
using System.Collections.Generic;
using System.Text;

namespace Sandwych.Reporting.Tests.Common
{
    public class Table
    {
        public Table()
        {
            this.Rows = new List<TableRow>
            {
                 new TableRow {
                     Text = "row1",
                     DateTime = DateTime.Now,
                     DateTimeOffset = DateTimeOffset.Now,
                     Decimal = 9990.00M,
                     Double = 9990.00,
                     Integer = 9990,
                     TimeSpan = new TimeSpan(12,12,32),
                     Integers = new int[] { 11,22,33,44,55},
                 },

                 new TableRow {
                     Text = "row2",
                     DateTime = DateTime.Now,
                     DateTimeOffset = DateTimeOffset.Now,
                     Decimal = 9991.00M,
                     Double = 9991.00,
                     Integer = 9991,
                     TimeSpan = new TimeSpan(23,12,32),
                     Integers = new int[] { 5,4,3,2,1},
                 },

                 new TableRow {
                     Text = "row3",
                     DateTime = DateTime.Now,
                     DateTimeOffset = DateTimeOffset.Now,
                     Decimal = 9992.00M,
                     Double = 9992.00,
                     Integer = 9992,
                     TimeSpan = new TimeSpan(18,12,32),
                     Integers = new int[] { 1,2,3,4,5},
                 },
            };
        }

        public IReadOnlyList<TableRow> Rows { get; private set; }

    }
}
