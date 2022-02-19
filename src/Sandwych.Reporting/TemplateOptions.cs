using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace Sandwych.Reporting
{
    public class TemplateOptions
    {
        public static TemplateOptions Default = new();

        public bool AllowUnsafeAccess { get; set; } = true;

        public CultureInfo CultureInfo { get; set; } = CultureInfo.InvariantCulture;

    }
}
