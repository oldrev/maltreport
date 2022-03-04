using System;
using System.Collections.Generic;
using System.Text;

namespace Sandwych.Reporting.Models
{
    public interface IMoney : IFormattable
    {
        decimal Value { get; }
        string CurrencyCode { get; }
    }
}
