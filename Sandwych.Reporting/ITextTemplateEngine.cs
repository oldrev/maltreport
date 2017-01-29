using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

using System.Diagnostics;
using System.Globalization;

namespace Sandwych.Reporting
{
	internal interface  ITextTemplateEngine
	{
		string LogTag { get; }

		void Evaluate (IDictionary<string, object> context, TextReader input, TextWriter output);

		void RegisterFilter (Type t, IRenderFilter filter);

		void Reset ();
     
	}
}
