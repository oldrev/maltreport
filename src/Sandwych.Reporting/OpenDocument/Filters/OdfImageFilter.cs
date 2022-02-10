using Fluid;
using Fluid.Values;
using Sandwych.Reporting.OpenDocument.Values;
using System;
using System.Threading.Tasks;

namespace Sandwych.Reporting.OpenDocument.Filters
{
    public class OdfImageFilter : IAsyncFilter
    {
        private readonly OdfDocument _document;

        public string Name => "image";

        public OdfImageFilter(OdfDocument odfDoc)
        {
            this._document = odfDoc;
        }

        public ValueTask<FluidValue> ExecuteAsync(FluidValue input, FilterArguments arguments, Fluid.TemplateContext context)
        {
            var blob = input.ToObjectValue() as ImageBlob;
            if (blob == null)
            {
                throw new NotSupportedException($"The property of your image must be a 'ImageBlob' type");
            }

            return new ValueTask<FluidValue>(new OdfImageBlobValue(_document, blob));
        }

    }
}