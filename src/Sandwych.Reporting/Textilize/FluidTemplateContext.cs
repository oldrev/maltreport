using System.Collections.Generic;

namespace Sandwych.Reporting.Textilize
{
    public class FluidTemplateContext : Fluid.TemplateContext
    {
        public FluidTemplateContext(TemplateContext docTempCtx)
        {
            this.Options.TimeZone = docTempCtx.Options.TimeZone;
            this.Options.MaxRecursion = docTempCtx.Options.MaxRecursion;
            this.Options.MaxSteps = docTempCtx.Options.MaxSteps;
            this.Options.Now = docTempCtx.Options.Now;
            this.Options.CultureInfo = docTempCtx.Options.CultureInfo;

            var fluidOptions = new Fluid.TemplateOptions()
            {
                CultureInfo = docTempCtx.Options.CultureInfo,
                Now = docTempCtx.Options.Now,
                MaxRecursion = docTempCtx.Options.MaxRecursion,
                MaxSteps = docTempCtx.Options.MaxSteps,
                TimeZone = docTempCtx.Options.TimeZone,
            };

            this.Options = fluidOptions;
            this.CultureInfo = fluidOptions.CultureInfo;
            this.TimeZone = fluidOptions.TimeZone;
            this.Captured = fluidOptions.Captured;
            this.Now = fluidOptions.Now;

            //TODO FIXME
            if (docTempCtx.Options.AllowUnsafeAccess)
            {
                this.Options.MemberAccessStrategy = Fluid.UnsafeMemberAccessStrategy.Instance;
            }
            foreach (var pair in docTempCtx.Values)
            {
                this.SetValue(pair.Key, Fluid.Values.FluidValue.Create(pair.Value, this.Options));
            }
        }
    }
}