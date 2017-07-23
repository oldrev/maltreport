using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Fluid;
using Fluid.Values;

namespace Sandwych.Reporting
{
    public interface IFilter
    {
        string Name { get; }
    }

    public interface ISyncFilter : IFilter
    {
        FluidValue Execute(FluidValue input, FilterArguments arguments, Fluid.TemplateContext context);
    }

    public interface IAsyncFilter : IFilter
    {
        Task<FluidValue> ExecuteAsync(FluidValue input, FilterArguments arguments, Fluid.TemplateContext context);
    }

}
