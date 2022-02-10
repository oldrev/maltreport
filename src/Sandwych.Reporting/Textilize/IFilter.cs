using Fluid;
using Fluid.Values;
using System.Threading.Tasks;

namespace Sandwych.Reporting
{
    public interface IFilter
    {
        string Name { get; }
    }

    public interface IAsyncFilter : IFilter
    {
        ValueTask<FluidValue> ExecuteAsync(FluidValue input, FilterArguments arguments, Fluid.TemplateContext context);
    }
}