using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;
using System.Text.Encodings.Web;
using System.Xml;
using Fluid.Values;
using Sandwych.Reporting.Textilize.Values;

namespace Sandwych.Reporting.Odf.Values
{
    public abstract class OdfTableCellValue : XmlOutputValue
    {
        public override FluidValues Type => this.Value.Type;

        private readonly Dictionary<string, string> _attributes = new Dictionary<string, string>();

        public IReadOnlyDictionary<string, string> Attributes => _attributes;

        public abstract string ValueType { get; }

        public OdfTableCellValue(FluidValue value) : base(value)
        {
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

        protected override void WriteTo(XmlWriter writer, TextEncoder encoder, CultureInfo cultureInfo)
        {
            writer.WriteStartElement("table-cell", "table");
            // write attributes
            foreach (var kp in _attributes)
            {
                var names = kp.Key.Split(':');
                writer.WriteAttributeString(names[1], names[0], kp.Value);
            }

            writer.WriteAttributeString("value-type", "office", this.ValueType);

            writer.WriteAttributeString("value-type", "calcext", this.ValueType);

            this.WriteTableCellAttributes(writer, encoder, cultureInfo);

            writer.WriteStartElement("text", "p");

            this.WriteTableCellText(writer, encoder, cultureInfo);

            writer.WriteEndElement();

            writer.WriteEndElement();
        }

        protected abstract void WriteTableCellAttributes(XmlWriter writer, TextEncoder encoder, CultureInfo cultureInfo);
        protected abstract void WriteTableCellText(XmlWriter writer, TextEncoder encoder, CultureInfo cultureInfo);

        public override int GetHashCode() => this.Value.GetHashCode();
    }


    public class OdfStringTableCellValue : OdfTableCellValue
    {
        public const string OdfValueType = "string";

        public OdfStringTableCellValue(FluidValue value) : base(value)
        {
        }

        public override string ValueType => OdfValueType;

        protected override void WriteTableCellAttributes(XmlWriter writer, TextEncoder encoder, CultureInfo cultureInfo)
        {
        }

        protected override void WriteTableCellText(XmlWriter writer, TextEncoder encoder, CultureInfo cultureInfo)
        {
            writer.WriteString(this.Value.ToStringValue().ToString());
        }
    }

    public class OdfBooleanTableCellValue : OdfTableCellValue
    {
        public const string OdfValueType = "boolean";

        public OdfBooleanTableCellValue(FluidValue value) : base(value)
        {
        }

        public override string ValueType => OdfValueType;

        protected override void WriteTableCellAttributes(XmlWriter writer, TextEncoder encoder, CultureInfo cultureInfo)
        {
            writer.WriteAttributeString("boolean-value", "office", this.Value.ToStringValue().ToString());
        }

        protected override void WriteTableCellText(XmlWriter writer, TextEncoder encoder, CultureInfo cultureInfo)
        {
            writer.WriteString(this.Value.ToBooleanValue().ToString(cultureInfo));
        }
    }

    public class OdfFloatTableCellValue : OdfTableCellValue
    {
        public const string OdfValueType = "float";

        public OdfFloatTableCellValue(FluidValue value) : base(value)
        {
        }

        public override string ValueType => OdfValueType;

        protected override void WriteTableCellAttributes(XmlWriter writer, TextEncoder encoder, CultureInfo cultureInfo)
        {
            writer.WriteAttributeString("value", "office", this.Value.ToStringValue().ToString());
        }

        protected override void WriteTableCellText(XmlWriter writer, TextEncoder encoder, CultureInfo cultureInfo)
        {
            writer.WriteString(this.Value.ToStringValue().ToString());
        }
    }

    public class OdfPercentageTableCellValue : OdfTableCellValue
    {
        public const string OdfValueType = "percentage";

        public OdfPercentageTableCellValue(FluidValue value) : base(value)
        {
        }

        public override string ValueType => OdfValueType;

        protected override void WriteTableCellAttributes(XmlWriter writer, TextEncoder encoder, CultureInfo cultureInfo)
        {
            writer.WriteAttributeString("value", "office", this.Value.ToStringValue().ToString());
        }

        protected override void WriteTableCellText(XmlWriter writer, TextEncoder encoder, CultureInfo cultureInfo)
        {
            var percentage = this.Value.ToNumberValue() * 100;
            writer.WriteString(percentage.ToString() + '%');
        }
    }

    public class OdfTimeTableCellValue : OdfTableCellValue
    {
        public const string OdfValueType = "time";

        public DateTimeOffset DateTimeValue { get; }

        public OdfTimeTableCellValue(DateTimeOffset value) : base(EmptyValue.Instance)
        {
            this.DateTimeValue = value;
        }

        public override string ValueType => OdfValueType;

        protected override void WriteTableCellAttributes(XmlWriter writer, TextEncoder encoder, CultureInfo cultureInfo)
        {
            var dt = this.DateTimeValue;
            var formattedTime = $"PT{dt.Hour}H{dt.Minute}M{dt.Second}S";
            writer.WriteAttributeString("time-value", "office", formattedTime);
        }

        protected override void WriteTableCellText(XmlWriter writer, TextEncoder encoder, CultureInfo cultureInfo)
        {
            var readableTimeText = this.DateTimeValue.ToString(cultureInfo);
            writer.WriteString(readableTimeText);
        }

    }

    public class OdfDateTableCellValue : OdfTableCellValue
    {
        public const string OdfValueType = "date";

        public DateTimeOffset DateTimeValue { get; }

        public OdfDateTableCellValue(DateTimeOffset value) : base(EmptyValue.Instance)
        {
            this.DateTimeValue = value;
        }

        public override string ValueType => OdfValueType;

        protected override void WriteTableCellAttributes(XmlWriter writer, TextEncoder encoder, CultureInfo cultureInfo)
        {
            var formattedDate = this.DateTimeValue.ToString("yyyy-M-dTH:m:s.FFFK", cultureInfo);
            writer.WriteAttributeString("date-value", "office", formattedDate);
        }

        protected override void WriteTableCellText(XmlWriter writer, TextEncoder encoder, CultureInfo cultureInfo)
        {
            var readableTimeText = this.DateTimeValue.ToString(cultureInfo);
            writer.WriteString(readableTimeText);
        }

    }
}
