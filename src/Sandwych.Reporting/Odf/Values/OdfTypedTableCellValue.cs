using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;
using System.Text.Encodings.Web;
using Fluid.Values;

namespace Sandwych.Reporting.Odf.Values
{
    public class OdfTypedTableCellValue : FluidValue
    {
        public override FluidValues Type => this.Value.Type;

        public FluidValue Value { get; }

        private readonly string _tableStyleName;

        public OdfTypedTableCellValue(FluidValue value, string tableStyleName)
        {
            this.Value = value;
            _tableStyleName = tableStyleName;
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

        public override void WriteTo(TextWriter writer, TextEncoder encoder, CultureInfo cultureInfo)
        {
            writer.Write("<table:table-cell ");
            if(_tableStyleName != null)
            {
                writer.Write(" table:style-name=\"");
                writer.Write(_tableStyleName);
                writer.Write("\" ");
            }
            writer.Write(" >");

            writer.Write("<text:p>");
            this.Value.WriteTo(writer, encoder, cultureInfo); 
            writer.Write("</text:p>");

            writer.Write("</table:table-cell>");
        }

        public override int GetHashCode() => this.Value.GetHashCode();
    }
}
