using System;
using System.Collections.Generic;
using System.Text;

namespace Sandwych.Reporting
{
    public interface ICompiler<TDocument, TTemplate>
        where TDocument : IDocument
        where TTemplate : ITemplate<TDocument>
    {
        TTemplate Compile(TDocument templateDocument);
    }
}
