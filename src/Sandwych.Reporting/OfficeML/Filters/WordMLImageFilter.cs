using System;
using Fluid;
using Fluid.Values;
using Sandwych.Reporting.Textilize;

namespace Sandwych.Reporting.OfficeML.Filters
{
    public struct WordMLImageFilter : ISyncFilter
    {
        public string Name => "image";

        public FluidValue Execute(FluidValue input, FilterArguments arguments, Fluid.TemplateContext context)
        {
            var buf = input.ToObjectValue() as byte[];
            if (buf == null)
            {
                return NilValue.Instance;
            }

            var base64 = Convert.ToBase64String(buf);
            return new StringValue(base64);
        }

    }
}