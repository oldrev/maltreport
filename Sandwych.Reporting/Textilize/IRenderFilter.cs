using System;
using System.Collections.Generic;
using System.Text;

namespace Sandwych.Reporting
{
    public interface IRenderFilter
    {
        string Filter(object originalValue);
    }
}
