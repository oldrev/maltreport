using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Fluid.Values;
using Sandwych.Reporting.Models;

namespace Sandwych.Reporting.Textilize.Values
{
    public class CurrencyValue : ObjectValueBase
    {
        public Currency Currency { get; }

        public CurrencyValue(object value) : base(value)
        {
        }

        public CurrencyValue(Currency currency) : base(currency) { }

        public override ValueTask<FluidValue> GetIndexAsync(FluidValue index, Fluid.TemplateContext context)
        {
            return Create(((Currency)Value).Code + "!!!" + index.ToStringValue(), context.Options);
        }
    }
}
