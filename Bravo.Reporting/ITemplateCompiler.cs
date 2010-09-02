using System;
using System.Collections.Generic;
using System.Text;

namespace Bravo.Reporting
{
    public interface ITemplateCompiler
    {
        ITemplate Compile(IDocument doc);
    }
}
