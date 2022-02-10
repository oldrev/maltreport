using Fluid;
using Fluid.Values;
using Sandwych.Reporting.OpenDocument.Values;
using System;
using System.Threading.Tasks;

namespace Sandwych.Reporting.OfficeML.Filters
{
    public struct WordMLImageFilter : IAsyncFilter
    {
        public string Name => "image";

        public ValueTask<FluidValue> ExecuteAsync(FluidValue input, FilterArguments arguments, Fluid.TemplateContext context)
        {
            var blob = input.ToObjectValue() as ImageBlob;
            if (blob == null)
            {
                return NilValue.Instance;
            }

            var base64 = Convert.ToBase64String(blob.GetBuffer());
            return new ValueTask<FluidValue>(new StringValue(base64));
        }

    }
}