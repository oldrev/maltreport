using System;
using System.Collections.Generic;
using System.Text;

namespace Sandwych.Reporting
{
    public interface ITemplateCompiler<TDocument> where TDocument : IDocument
    {
        ITemplate CompileAsync(IDocument doc);
    }
}
