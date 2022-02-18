using Fluid;
using System.Collections.Generic;

namespace Sandwych.Reporting.Textilize
{
    public class FluidTemplateContext : Fluid.TemplateContext
    {

        public FluidTemplateContext(IReadOnlyDictionary<string, object> context, bool allowUnsafe = true)
        {
            //TODO FIXME
            if (allowUnsafe)
            {
                this.Options.MemberAccessStrategy = UnsafeMemberAccessStrategy.Instance;
            }
            foreach (var pair in context)
            {
                this.SetValue(pair.Key, Fluid.Values.FluidValue.Create(pair.Value, this.Options));
                /*
                if (pair.Value != null)
                {
                    this.Options.MemberAccessStrategy.Register(pair.Value.GetType());
                }
                */
            }
        }
    }
}