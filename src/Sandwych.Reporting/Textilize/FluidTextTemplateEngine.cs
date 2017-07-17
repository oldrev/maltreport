using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Fluid;

namespace Sandwych.Reporting.Textilize
{
    public class FluidTextTemplateEngine : ITextTemplateEngine
    {
        public string LogTag => throw new NotImplementedException();

        public void Evaluate(IDictionary<string, object> context, TextReader input, TextWriter output)
        {
            throw new NotImplementedException();
        }

        public void RegisterFilter(Type t, IRenderFilter filter)
        {
            throw new NotImplementedException();
        }

    }
}
