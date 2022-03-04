using Fluid.Values;
using System;
using System.IO;
using System.Text.Encodings.Web;
using System.Globalization;
using Sandwych.Reporting.Textilize.Values;
using System.Xml;

namespace Sandwych.Reporting.Odf.Values
{
    public class OdfImageBlobValue : XmlOutputValue
    {
        public DocumentBlobEntry DocumentBlobEntry { get; }

        public OdfImageBlobValue(FluidValue input, DocumentBlobEntry entry) : base(input)
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

        protected override void WriteTo(XmlWriter writer, TextEncoder encoder, CultureInfo cultureInfo)
        {
            var imagePath = this.DocumentBlobEntry.EntryPath;
            writer.WriteStartElement("image", "draw");
            writer.WriteAttributeString("href", "xlink", imagePath);
            writer.WriteAttributeString("type", "xlink", "simple");
            writer.WriteAttributeString("show", "xlink", "embed");
            writer.WriteAttributeString("actuate", "xlink", "onLoad");
            writer.WriteEndElement();
        }
    }
}