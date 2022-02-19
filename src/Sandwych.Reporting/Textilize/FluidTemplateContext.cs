using System.Collections.Generic;

namespace Sandwych.Reporting.Textilize
{
    public class FluidTemplateContext : Fluid.TemplateContext
    {
        public FluidTemplateContext(IReadOnlyDictionary<string, object> values, TemplateOptions docTemplateOptions)
        {
            //TODO FIXME
            if (docTemplateOptions.AllowUnsafeAccess)
            {
                this.Options.MemberAccessStrategy = Fluid.UnsafeMemberAccessStrategy.Instance;
            }
            foreach (var pair in values)
            {
                this.SetValue(pair.Key, Fluid.Values.FluidValue.Create(pair.Value, this.Options));
            }
        }
    }
}