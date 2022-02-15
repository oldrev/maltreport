using Fluid;
using Fluid.Values;
using System.Threading.Tasks;

namespace Sandwych.Reporting.Textilize
{
    public interface IFluidFilter 
    {
        string Name { get; }
        ValueTask<FluidValue> InvokeAsync(FluidValue input, FilterArguments arguments, Fluid.TemplateContext context);
    }
}