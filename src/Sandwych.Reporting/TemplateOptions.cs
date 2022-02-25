using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace Sandwych.Reporting
{
    public class TemplateOptions
    {
        public static TemplateOptions Default = new();

        public bool AllowUnsafeAccess { get; set; } = false;

        /// <summary>
        /// Gets or sets the maximum number of steps a script can execute. Leave to 0 for unlimited.
        /// </summary>
        public int MaxSteps { get; set; } = 0;

        /// <summary>
        /// Gets or sets the <see cref="CultureInfo"/> instance used to render locale values like dates and numbers.
        /// </summary>
        public CultureInfo CultureInfo { get; set; } = CultureInfo.InvariantCulture;

        /// <summary>
        /// Gets or sets the value returned by the "now" keyword.
        /// </summary>
        public Func<DateTimeOffset> Now { get; set; } = static () => DateTimeOffset.Now;

        /// <summary>
        /// Gets or sets the local time zone used when parsing or creating dates without specific ones.
        /// </summary>
        public TimeZoneInfo TimeZone { get; set; } = TimeZoneInfo.Local;

        /// <summary>
        /// Gets or sets the maximum depth of recursions a script can execute. 100 by default.
        /// </summary>
        public int MaxRecursion { get; set; } = 100;
    }
}
