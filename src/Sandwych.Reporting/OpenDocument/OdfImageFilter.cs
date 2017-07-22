using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;
using System.Xml;
using System.IO;
using System.Globalization;
using Fluid.Values;
using Fluid;

namespace Sandwych.Reporting.OpenDocument
{
    internal class OdfImageFilter : IFilter
    {
        private OdfDocument _outputDocument;

        public OdfImageFilter(OdfDocument odfDoc)
        {
            this._outputDocument = odfDoc;
        }

        public FluidValue Execute(FluidValue input, FilterArguments arguments, TemplateContext context)
        {
            var blob = input.ToObjectValue() as Blob;
            if (blob == null)
            {
                throw new NotSupportedException($"The image property must be a Blob type, but we got '{input.ToObjectValue().GetType().Name}'");
            }

            return new OdfImageBlobValue(this._outputDocument, blob);
        }

    }
}
