using System;
using System.Collections.Generic;
using System.Text;

namespace Bravo.Reporting
{
    internal interface IRenderFilter
    {
        object Filter(object originalValue);
    }
}
