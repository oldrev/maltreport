using Sandwych.Reporting.Textilize;
using System.Collections.Generic;

namespace Sandwych.Reporting
{
    public class TemplateContext
    {
        private readonly FluidTemplateContext _fluidContext;

        public FluidTemplateContext FluidContext => _fluidContext;

        public TemplateContext(
            IReadOnlyDictionary<string, object> values,
            IEnumerable<ISyncFilter> syncFilters = null, IEnumerable<IAsyncFilter> asyncFilters = null)
        {
            _fluidContext = new FluidTemplateContext(values);

            if (syncFilters != null)
            {
                foreach (var filter in syncFilters)
                {
                    _fluidContext.Filters.AddFilter(filter.Name, filter.Execute);
                }
            }

            if (asyncFilters != null)
            {
                foreach (var filter in asyncFilters)
                {
                    _fluidContext.Filters.AddAsyncFilter(filter.Name, filter.ExecuteAsync);
                }
            }
        }
    }
}