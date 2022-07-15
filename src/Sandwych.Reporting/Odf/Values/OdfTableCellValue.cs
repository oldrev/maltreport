using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;
using System.Text.Encodings.Web;
using Fluid.Values;

namespace Sandwych.Reporting.Odf.Values
{
    public abstract class OdfTableCellValue : FluidValue
    {
        public override FluidValues Type => this.Value.Type;

        public FluidValue Value { get; }

        private readonly Dictionary<string, string> _attributes = new Dictionary<string, string>();

        public IReadOnlyDictionary<string, string> Attributes => _attributes;

        public abstract string ValueType { get; }

        public OdfTableCellValue(FluidValue value)
        {
            this.Value = value;
        }

        public void AddAttribute(string attribute, string value)
        {
            _attributes.Add(attribute, value);
        }

        public override bool Equals(FluidValue other)
        {
            if (other == BlankValue.Instance || other == NilValue.Instance || other == EmptyValue.Instance)
            {
                return false;
            }

            return this.Value == other;
        }

        public bool Equals(OdfTableCellValue other)
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

            // write attributes
            foreach (var kp in _attributes)
            {
                writer.Write(" ");
                writer.Write(kp.Key);
                writer.Write("=");
                writer.Write("\"");
                writer.Write(kp.Value);
                writer.Write("\" ");
            }

            writer.Write(" office:value-type=\"");
            writer.Write(this.ValueType);
            writer.Write("\" ");

            writer.Write(" calcext:value-type=\"");
            writer.Write(this.ValueType);
            writer.Write("\" ");

            this.WriteValueAndTypeAttributes(writer, encoder, cultureInfo);

            writer.Write(" >");

            writer.Write("<text:p>");
            this.Value.WriteTo(writer, encoder, cultureInfo);
            writer.Write("</text:p>");

            writer.Write("</table:table-cell>");
        }

        protected abstract void WriteValueAndTypeAttributes(TextWriter writer, TextEncoder encoder, CultureInfo cultureInfo);

        public override int GetHashCode() => this.Value.GetHashCode();
    }


    public class OdfStringTableCellValue : OdfTableCellValue
    {
        public const string OdfValueType = "string";

        public OdfStringTableCellValue(FluidValue value) : base(value)
        {
        }

        public override string ValueType => OdfValueType;

        protected override void WriteValueAndTypeAttributes(TextWriter writer, TextEncoder encoder, CultureInfo cultureInfo)
        {
            writer.Write(" office:value=\"");
            this.Value.WriteTo(writer, encoder, cultureInfo);
            writer.Write("\" ");
        }
    }

    public class OdfFloatTableCellValue : OdfTableCellValue
    {
        public const string OdfValueType = "float";

        public OdfFloatTableCellValue(FluidValue value) : base(value)
        {
        }

        public override string ValueType => OdfValueType;

        protected override void WriteValueAndTypeAttributes(TextWriter writer, TextEncoder encoder, CultureInfo cultureInfo)
        {
            writer.Write(" office:value=\"");
            this.Value.WriteTo(writer, encoder, cultureInfo);
            writer.Write("\" ");
        }
    }

    public class OdfPercentageTableCellValue : OdfTableCellValue
    {
        public const string OdfValueType = "percentage";

        public OdfPercentageTableCellValue(FluidValue value) : base(value)
        {
        }

        public override string ValueType => OdfValueType;

        protected override void WriteValueAndTypeAttributes(TextWriter writer, TextEncoder encoder, CultureInfo cultureInfo)
        {
            writer.Write(" office:value=\"");
            this.Value.WriteTo(writer, encoder, cultureInfo);
            writer.Write("\" ");
        }
    }

    public class OdfTimeTableCellValue : OdfTableCellValue
    {
        public const string OdfValueType = "time";

        public OdfTimeTableCellValue(FluidValue value, Fluid.TemplateContext context) : base(FluidValue.Create(value, context.Options))
        {
        }

        public override string ValueType => OdfValueType;

        protected override void WriteValueAndTypeAttributes(TextWriter writer, TextEncoder encoder, CultureInfo cultureInfo)
        {
            if(!this.Value.TryGetDateTimeInput(null, out var dt))
            {
                throw new IOException($"Can not parse the input time value: '{this.Value.ToStringValue()}'");
            }

            // PT21H42M02.164S
            writer.Write($" office:time-value=\"PT{dt.Hour}H{dt.Minute}M{dt.Second}.{dt.Millisecond}S");
            this.Value.WriteTo(writer, encoder, cultureInfo);
            writer.Write("\" ");
        }
    }
}
