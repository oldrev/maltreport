using System;
using System.Collections.Generic;
using System.Text;
using Fluid;

namespace Sandwych.Reporting.Textilize
{
    public interface IFluidTemplateContextFactory
    {
        TemplateContext CreateTemplateContext();
    }
}
