using Sandwych.Reporting.Textilize;
using System.Collections.Generic;
using System.Globalization;

namespace Sandwych.Reporting
{
    public class TemplateContext
    {
        public TemplateContext(IReadOnlyDictionary<string, object> values)
        {
            this.Culture = CultureInfo.CurrentCulture;
            this.Values = values;
        }

        public IReadOnlyDictionary<string, object> Values { get; private set; }

        public CultureInfo Culture { get; set; }
    }
}