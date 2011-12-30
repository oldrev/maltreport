using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace Malt.Reporting
{
    public interface ITemplate : IDocument
    {
        void Compile();
        IDocument Render(IDictionary<string, object> context);
    }
}
