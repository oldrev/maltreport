using System;
using System.Collections.Generic;
using System.Text;

namespace Sandwych.Reporting
{
	internal sealed class VelocityEscapeTool
	{
		private static readonly VelocityEscapeTool s_instance = new VelocityEscapeTool ();
		public const string DefaultKey = "esc";

		public static char D { get { return '$'; } }

		public static char H { get { return '#'; } }

		public static string EscapeVelocity (string str)
		{
			if (str == null) {
				throw new ArgumentNullException ("str");
			}

			return EscapeDirective (str.Replace ("$", "${" + DefaultKey + ".d}"));
		}

		public static string EscapeDirective (string str)
		{
			if (str == null) {
				throw new ArgumentNullException ("str");
			}

			return str.Replace ("#", "${" + DefaultKey + ".h}");
		}

		public static VelocityEscapeTool Instance { get { return s_instance; } }
	}
}
