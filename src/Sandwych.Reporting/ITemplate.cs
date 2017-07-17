using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using Fluid;

namespace Sandwych.Reporting
{
    public interface ITemplate
    {
        IDocument Render(IReadOnlyDictionary<string, object> context);
    }
}
