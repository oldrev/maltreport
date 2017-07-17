using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
