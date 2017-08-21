using System;
using System.Collections.Generic;
using System.Text;
using Fluid;
using Fluid.Values;

namespace Sandwych.Reporting.Textilize.Filters
{
    public struct ByteArrayToBase64Filter : ISyncFilter
    {
        public string Name => "base64";

        public FluidValue Execute(FluidValue input, FilterArguments arguments, Fluid.TemplateContext context)
        {
            var buf = input.ToObjectValue() as byte[];

            if (buf == null)
            {
                throw new NotSupportedException($"The type of input value must be 'byte[]'");
            }

            var base64Str = Convert.ToBase64String(buf);
            return new StringValue(base64Str);
        }
    }
}
