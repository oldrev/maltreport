using Fluid.Values;
using System;
using System.IO;
using System.Text.Encodings.Web;
using System.Globalization;

namespace Sandwych.Reporting.Odf.Values
{
    public class OdfImageBlobValue : FluidValue
    {
        public DocumentBlobEntry DocumentBlobEntry { get; }

        public OdfImageBlobValue(DocumentBlobEntry entry)
        {
            this.DocumentBlobEntry = entry;
        }

        public override FluidValues Type => throw new NotImplementedException();

        public override bool Equals(FluidValue other)
        {
            if (other is OdfImageBlobValue otherImageValue)
            {
                return otherImageValue.DocumentBlobEntry.Equals(this.DocumentBlobEntry.Blob);
            }

            return false;
        }

        public override bool ToBooleanValue()
        {
            return true;
        }

        public override decimal ToNumberValue()
        {
            return 0;
        }

        public override object ToObjectValue()
        {
            return this.DocumentBlobEntry.Blob;
        }

        public override string ToStringValue()
        {
            return string.Empty;
        }

        public override void WriteTo(TextWriter writer, TextEncoder encoder, CultureInfo cultureInfo)
        {
            var imagePath = this.DocumentBlobEntry.EntryPath;
            writer.Write(
                $"<draw:image xlink:href=\"{imagePath}\" xlink:type=\"simple\" xlink:show=\"embed\" xlink:actuate=\"onLoad\" />");
        }
    }
}