using Fluid;
using Fluid.Values;
using Sandwych.Reporting.Odf.Values;
using Sandwych.Reporting.Textilize;
using System;
using System.Threading.Tasks;

namespace Sandwych.Reporting.Odf.Filters
{
    public class OdfImageFilter : IFluidFilter
    {
        private readonly OdfDocument _document;

        public const string FilterName = "__ODF_Filters_Image";

        public string Name => FilterName;

        public OdfImageFilter(OdfDocument odfDoc)
        {
            _document = odfDoc;
        }

        public ValueTask<FluidValue> InvokeAsync(FluidValue input, FilterArguments arguments, Fluid.TemplateContext context)
        {
            var blob = input.ToObjectValue() as ImageBlob;
            if (blob == null)
            {
                throw new NotSupportedException($"The property of your image must be a 'ImageBlob' type");
            }

            var blobEntry = _document.AddOrGetImageEntry(blob);

            return new ValueTask<FluidValue>(new OdfImageBlobValue(blobEntry));
        }

    }
}