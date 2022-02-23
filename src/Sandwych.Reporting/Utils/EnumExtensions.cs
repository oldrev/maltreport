using System;
using System.Collections.Generic;
using System.Text;

namespace Sandwych.Reporting.Utils
{
    public static class EnumExtensions
    {
        public static string EnumGetName<TEnum>(this TEnum enumValue) =>
            Enum.GetName(typeof(TEnum), enumValue);
    }
}
