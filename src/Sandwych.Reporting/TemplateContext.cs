using Sandwych.Reporting.Textilize;
using System.Collections.Generic;

namespace Sandwych.Reporting
{
    public class TemplateContext
    {
        public TemplateContext(IReadOnlyDictionary<string, object> values)
        {
            this.Values = values;
        }

        public IReadOnlyDictionary<string, object> Values { get; private set; }

    }
}