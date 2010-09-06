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
            if (str == null)
            {
                throw new ArgumentNullException("str");
            }

            return EscapeDirective(str.Replace("$", "${" + DefaultKey + ".d}"));
        }

        public static string EscapeDirective(string str)
        {
            if (str == null)
            {
                throw new ArgumentNullException("str");
            }

            return str.Replace("#", "${" + DefaultKey + ".h}");
        }
    }
}
