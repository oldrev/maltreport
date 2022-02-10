using Fluid.Values;
using System;
using System.IO;
using System.Text.Encodings.Web;
using System.Globalization;

namespace Sandwych.Reporting.OpenDocument.Values
{
    public class OdsTableCellDataValue : FluidValue, IEquatable<OdsTableCellDataValue>
    {
        private readonly string _cellFormat;
        private readonly object _value;

        public OdsTableCellDataValue(string cellFormat, object value)
        {
            _cellFormat = cellFormat;
            _value = value;
            this.Type = FluidValues.Array;
        }

        public override FluidValues Type { get; }

        public override bool Equals(FluidValue other)
        {
            if (other == NilValue.Instance)
            {
                return _value == null;
            }

            if (other is OdsTableCellDataValue)
            {
                return this.Equals(other);
            }

            return false;
        }

        public override bool ToBooleanValue()
        {
            throw new NotSupportedException();
        }

        public override decimal ToNumberValue()
        {
            throw new NotSupportedException();
        }

        public override object ToObjectValue()
        {
            throw new NotSupportedException();
        }

        public override string ToStringValue()
        {
            throw new NotSupportedException();
        }

        public override void WriteTo(TextWriter writer, TextEncoder encoder, CultureInfo cultureInfo)
        {
            throw new NotImplementedException();
        }

        public bool Equals(OdsTableCellDataValue other)
        {
            throw new NotImplementedException();
        }
    }
}