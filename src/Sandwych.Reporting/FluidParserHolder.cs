using System;
using System.Collections.Generic;
using System.Text;
using Fluid;

namespace Sandwych.Reporting
{
    public static class FluidParserHolder
    {
        private static Lazy<FluidParser> _parser = new Lazy<FluidParser>(() =>
        {
            var parser = new FluidParser();
            parser.Compile();
            return parser;
        }, true);

        public static FluidParser Instance => _parser.Value;
    }
}
