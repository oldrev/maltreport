using System;
using Fluid.Values;
using Fluid;
using Sandwych.Reporting.OpenDocument.Values;

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
            var buf = input.ToObjectValue() as byte[];
            if (buf == null)
            {
                throw new NotSupportedException($"The property of your image must be a 'byte[]' type");
            }

            if (arguments.Count != 1 || arguments.At(0).Type != FluidValues.String)
            {
                throw new SyntaxErrorException("The image filter must have a format argument, try like \"dtl://yourObject.imageProperty | image: 'jpeg'\"");
            }

            var imageFormat = arguments.At(0);

            var blob = new ImageBlob(imageFormat.ToStringValue(), buf);

            return new OdfImageBlobValue(this._outputDocument, blob);
        }

    }
}
