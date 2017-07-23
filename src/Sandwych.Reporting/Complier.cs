using System;
using System.Collections.Generic;
using System.Text;

namespace Sandwych.Reporting
{
    public interface IComplier<TDocument>
        where TDocument : IDocument
    {
        ITemplate<TDocument> Compile(TDocument templateDocument);
    }
}
