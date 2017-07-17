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
                     TimeSpan = new TimeSpan(12,12,32)
                 },

                 new TableRow {
                     Text = "row2",
                     DateTime = DateTime.Now,
                     DateTimeOffset = DateTimeOffset.Now,
                     Decimal = 9991.00M,
                     Double = 9991.00,
                     Integer = 9991,
                     TimeSpan = new TimeSpan(23,12,32)
                 },

                 new TableRow {
                     Text = "row3",
                     DateTime = DateTime.Now,
                     DateTimeOffset = DateTimeOffset.Now,
                     Decimal = 9992.00M,
                     Double = 9992.00,
                     Integer = 9992,
                     TimeSpan = new TimeSpan(18,12,32)
                 },
            };
        }

        public IReadOnlyList<TableRow> Rows { get; private set; }

    }
}
