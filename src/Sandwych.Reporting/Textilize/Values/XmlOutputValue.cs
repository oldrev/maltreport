using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;
using System.Text.Encodings.Web;
using System.Xml;
using Fluid.Values;

namespace Sandwych.Reporting.Textilize.Values
{
    public abstract class XmlOutputValue : FluidValue
    {
        public FluidValue Value { get; }

        public XmlOutputValue(FluidValue value)
        {
            this.Value = value;
        }

        protected abstract void WriteTo(XmlWriter writer, TextEncoder encoder, CultureInfo cultureInfo);

        public override void WriteTo(TextWriter writer, TextEncoder encoder, CultureInfo cultureInfo)
        {
            using var xw = XmlWriter.Create(writer);
            this.WriteTo(xw, encoder, cultureInfo);
        }

        public override bool Equals(FluidValue other)
        {
            if (other == BlankValue.Instance || other == NilValue.Instance || other == EmptyValue.Instance)
            {
                return false;
            }

            return this.Value.Equals(other);
        }

        public bool Equals(XmlOutputValue other)
        {
            if (other is null)
            {
                return false;
            }

            if (ReferenceEquals(this, other))
            {
                return true;
            }

            return this.Value.Equals(other.Value);
        }

        public override bool ToBooleanValue() => this.Value.ToBooleanValue();

        public override decimal ToNumberValue() => this.Value.ToNumberValue();

        public override object ToObjectValue() => this.Value.ToObjectValue();

        public override string ToStringValue() => this.Value.ToStringValue();


    }
}
