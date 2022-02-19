using Fluid.Values;
using Sandwych.Reporting.Odf.Values;
using System;
using System.IO;
using System.Text.Encodings.Web;
using System.Globalization;

namespace Sandwych.Reporting.Odf
{
    public class FluidBlobValue : FluidValue
    {
        private readonly Blob _value;
        public Blob Blob => _value;

        public FluidBlobValue(Blob blob)
        {
            _value = blob;
        }

        public override FluidValues Type => throw new NotImplementedException();

        public override bool Equals(FluidValue other)
        {
            if (other == NilValue.Instance)
            {
                return _value == null;
            }

            if (other is FluidBlobValue otherBlobValue)
            {
                return otherBlobValue.Blob.Equals(this.Blob);
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
            return _value;
        }

        public override string ToStringValue()
        {
            return string.Empty;
        }

        public override void WriteTo(TextWriter writer, TextEncoder encoder, CultureInfo cultureInfo)
        {
            throw new NotSupportedException("The FluidBlobValue must co-operate with a customized filter");
        }
    }
}