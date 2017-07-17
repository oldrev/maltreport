using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sandwych.Reporting.Textilize
{
    public class DefaultIsoDateTimeOffsetRenderFilter : IRenderFilter
    {
        public string Filter(object originalValue)
        {
            if (!(originalValue is DateTimeOffset))
            {
                throw new ArgumentOutOfRangeException(nameof(originalValue));
            }

            var dto = (DateTimeOffset)originalValue;
            return dto.ToString("s");
        }
    }
}
