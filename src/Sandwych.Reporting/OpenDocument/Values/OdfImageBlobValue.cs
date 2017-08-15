using Fluid.Values;
using System;
using System.IO;
using System.Text.Encodings.Web;
using System.Globalization;

namespace Sandwych.Reporting.OpenDocument.Values
{
    public class OdfImageBlobValue : FluidValue
    {
        private readonly ImageBlob _value;
        private readonly OdfDocument _outputDocument;

        public ImageBlob Blob => _value;

        public OdfImageBlobValue(OdfDocument outputDocument, ImageBlob blob)
        {
            _outputDocument = outputDocument;
            _value = blob;
        }

        public override FluidValues Type => throw new NotImplementedException();

        public override bool Equals(FluidValue other)
        {
            if (other == NilValue.Instance)
            {
                return _value == null;
            }

            if (other is OdfImageBlobValue otherImageValue)
            {
                return otherImageValue.Blob.Equals(this.Blob);
            }

            return false;
        }

        public override bool ToBooleanValue()
        {
            return true;
        }

        public override double ToNumberValue()
        {
            return 0;
        }

        public override object ToObjectValue()
        {
            return _value;
        }

        public override string ToStringValue()
        {
            return string.Empty;
        }

        public override void WriteTo(TextWriter writer, TextEncoder encoder, CultureInfo cultureInfo)
        {
            var blobEntry = _outputDocument.AddOrGetImage(_value);

            this.WriteDrawImageElement(writer, blobEntry.EntryPath);
        }

        private void WriteDrawImageElement(TextWriter tw, string imagePath)
        {
            tw.Write($"<draw:image xlink:href=\"{imagePath}\" xlink:type=\"simple\" xlink:show=\"embed\" xlink:actuate=\"onLoad\" />");
        }
    }
}