using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;
using System.Text.Encodings.Web;
using Fluid.Values;

namespace Sandwych.Reporting.Odf.Values
{
    public class OdfTypedValue : FluidValue
    {
        public override FluidValues Type => this.Value.Type;

        public FluidValue Value { get; }

        public string ValueType { get; }

        public OdfTypedValue(FluidValue value, string valueType)
        {
            this.Value = value;
            this.ValueType = valueType;
        }

        public override bool Equals(FluidValue other)
        {
            if (other == BlankValue.Instance || other == NilValue.Instance || other == EmptyValue.Instance)
            {
                return false;
            }

            return this.Value == other;
        }

        public bool Equals(OdfTypedValue other)
        {
            if (other is null)
            {
                return false;
            }

            if (ReferenceEquals(this, other))
            {
                return true;
            }

            return this.Value == other.Value;
        }

        public override bool ToBooleanValue() => this.Value.ToBooleanValue();

        public override decimal ToNumberValue() => this.Value.ToNumberValue();

        public override object ToObjectValue() => this.Value.ToObjectValue();

        public override string ToStringValue() => this.Value.ToStringValue();

        public override void WriteTo(TextWriter writer, TextEncoder encoder, CultureInfo cultureInfo) =>
            this.Value.WriteTo(writer, encoder, cultureInfo); 

        public override int GetHashCode() => this.Value.GetHashCode();
    }
}
