using System.Globalization;

namespace Sandwych.Reporting
{
    public class Configuration
    {
        public Configuration()
        {
            this.CultureInfo = CultureInfo.CurrentCulture;
        }

        public CultureInfo CultureInfo { get; set; }
    }
}