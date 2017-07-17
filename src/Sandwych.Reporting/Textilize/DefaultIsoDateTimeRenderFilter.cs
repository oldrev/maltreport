using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sandwych.Reporting.Textilize
{
    public class DefaultIsoDateTimeRenderFilter : IRenderFilter
    {
        public string Filter(object originalValue)
        {
            if (!(originalValue is DateTime))
            {
                throw new ArgumentOutOfRangeException(nameof(originalValue));
            }

            var dt = (DateTime)originalValue;
            return dt.ToString("s");
        }
    }
}
