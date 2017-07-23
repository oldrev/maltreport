using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;
using System.Text.Encodings.Web;
using System.Xml;
using Fluid;
using Fluid.Values;
using Sandwych.Reporting.OpenDocument.Values;

namespace Sandwych.Reporting.OpenDocument
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
            if (other == EmptyValue.Instance)
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

        public override void WriteTo(TextWriter writer, TextEncoder encoder)
        {
            throw new NotSupportedException("The FluidBlobValue must co-operate with a customized filter");
        }
    }
}
