using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Linq;

namespace Sandwych.Reporting.Models
{
    public struct Money : IMoney, IEquatable<IMoney>, IEquatable<Money>
    {
        public decimal Value { get; }
        public string CurrencyCode { get; }

        public Money(decimal value, string code)
        {
            this.Value = value;
            this.CurrencyCode = code;
        }

        public string ToString(string format, IFormatProvider formatProvider) =>
            this.Value.ToString(format, formatProvider);

        public bool Equals(IMoney other)
        {
            if (ReferenceEquals(null, other)) { return false; }

            return this.Value.Equals(other.Value) && this.CurrencyCode.Equals(other.CurrencyCode);
        }

        public bool Equals(Money other) =>
            this.Value.Equals(other.Value) && this.CurrencyCode.Equals(other.CurrencyCode);

        public override bool Equals(object obj)
        {
            if (obj == null) { return false; }
            if (obj.GetType() != typeof(Money)) { return false; }
            if (this.Equals(obj)) { return true; }
            return false;
        }

        public override string ToString() => base.ToString();

        public override int GetHashCode() =>
            this.Value.GetHashCode() ^ this.CurrencyCode.GetHashCode();
    }
}
