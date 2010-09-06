using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace Bravo.Reporting
{
    public interface ITemplate : IDocument
    {
        IDocument Render(IDictionary<string, object> context);
    }
}
