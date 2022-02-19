using System.Collections.Generic;
using System.Globalization;
using Sandwych.Reporting.Textilize;

namespace Sandwych.Reporting
{
    public class TemplateContext
    {
        public TemplateContext(IReadOnlyDictionary<string, object> values, TemplateOptions options = default)
        {
            this.Values = values;
            this.Options = options ?? TemplateOptions.Default;
        }

        public IReadOnlyDictionary<string, object> Values { get;  }

        public TemplateOptions Options { get; }

        public CultureInfo CultureInfo => this.Options.CultureInfo;

        public bool AllowUnsafeAccess => this.Options.AllowUnsafeAccess;

    }
}