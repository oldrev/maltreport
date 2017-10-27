using Fluid;
using Fluid.Values;
using Sandwych.Reporting.OpenDocument.Values;
using System;

namespace Sandwych.Reporting.OpenDocument.Filters
{
    public struct OdfImageFilter : ISyncFilter
    {
        private OdfDocument _outputDocument;

        public string Name => "image";

        public OdfImageFilter(OdfDocument odfDoc)
        {
            this._outputDocument = odfDoc;
        }

        public FluidValue Execute(FluidValue input, FilterArguments arguments, Fluid.TemplateContext context)
        {
            var blob = input.ToObjectValue() as ImageBlob;
            if (blob == null)
            {
                throw new NotSupportedException($"The property of your image must be a 'ImageBlob' type");
            }

            var imageFormat = arguments.At(0);

            return new OdfImageBlobValue(this._outputDocument, blob);
        }
    }
}