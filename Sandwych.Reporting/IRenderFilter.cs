using System;
using System.Collections.Generic;
using System.Text;

namespace Sandwych.Reporting
{
	internal interface IRenderFilter
	{
		object Filter (object originalValue);
	}
}
