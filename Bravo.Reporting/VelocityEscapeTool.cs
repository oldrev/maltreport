using System;
using System.Collections.Generic;
using System.Text;

namespace Bravo.Reporting
{
    internal class VelocityEscapeTool
    {
        public const string DefaultKey = "esc";

        public char D { get { return '$'; } }

        public char H { get { return '#'; } }

        public static string EscapeVelocity(string str)
        {
            return str.Replace("$", "${" + DefaultKey + ".d}").Replace("#", "${" + DefaultKey + ".h}");
        }

    }
}
