using Fluid;
using Fluid.Values;
using Sandwych.Reporting.OpenDocument.Values;
using System;

namespace Sandwych.Reporting.OfficeML.Filters
{
    public struct WordMLImageFilter : ISyncFilter
    {
        public string Name => "image";

        public FluidValue Execute(FluidValue input, FilterArguments arguments, Fluid.TemplateContext context)
        {
            var blob = input.ToObjectValue() as ImageBlob;
            if (blob == null)
            {
                return NilValue.Instance;
            }

            var base64 = Convert.ToBase64String(blob.GetBuffer());
            return new StringValue(base64);
        }
    }
}