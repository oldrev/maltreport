using System;
using System.Collections.Generic;
using System.Text;

namespace Sandwych.Reporting.Models
{

    public struct Currency 
    {
        public string Code { get; }
        public decimal Value { get; }

        public Currency(decimal value, string code)
        {
            this.Value = value;
            this.Code = code ?? throw new ArgumentNullException(nameof(code));
        }

        public override bool Equals(object obj) =>
            obj is Currency && this.Equals((Currency)obj);

        public bool Equals(Currency other) =>
            this.Value == other.Value && this.Code.Equals(other.Code);

        public override int GetHashCode() =>
            this.Value.GetHashCode() ^ this.Code.GetHashCode();
    }

}
