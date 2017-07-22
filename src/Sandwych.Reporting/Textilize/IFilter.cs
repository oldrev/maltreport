using System;
using System.Collections.Generic;
using System.Text;
using Fluid;
using Fluid.Values;

namespace Sandwych.Reporting
{
    public interface IFilter
    {
        FluidValue Execute(FluidValue input, FilterArguments arguments, TemplateContext context);
    }
}
