using Fluid;
using Fluid.Values;
using Sandwych.Reporting.OfficeML.Values;
using System;

namespace Sandwych.Reporting.OfficeML.Filters
{
    public struct WordMLImageFilter : ISyncFilter
    {
        private WordMLDocument _outputDocument;

        public string Name => "image";

        public WordMLImageFilter(WordMLDocument odfDoc)
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

            return new WordMLImageBlobValue(this._outputDocument, blob);
        }
    }
}