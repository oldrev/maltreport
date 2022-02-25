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

        public override FluidValues Type => FluidValues.Object;

        public override bool Equals(FluidValue other)
        {
            if (ReferenceEquals(other, null))
            {
                return false;
            }

            if (ReferenceEquals(other, this))
            {
                return true;
            }

            if (other is OdfImageBlobValue otherImageValue)
            {
                this.Equals(otherImageValue);
            }

            return false;
        }

        public bool Equals(OdfImageBlobValue other) => other.DocumentBlobEntry.Equals(this.DocumentBlobEntry.Blob);

        public override int GetHashCode() =>
            this.DocumentBlobEntry.GetHashCode();

        public override bool ToBooleanValue() => true;

        public override decimal ToNumberValue() => 0;

        public override object ToObjectValue() => this.DocumentBlobEntry;

        public override string ToStringValue() => this.DocumentBlobEntry.ToString();

        public override void WriteTo(TextWriter writer, TextEncoder encoder, CultureInfo cultureInfo)
        {
            var imagePath = this.DocumentBlobEntry.EntryPath;
            writer.Write(
                $"<draw:image xlink:href=\"{imagePath}\" xlink:type=\"simple\" xlink:show=\"embed\" xlink:actuate=\"onLoad\" />");
        }
    }
}