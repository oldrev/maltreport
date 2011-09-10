using System;
using System.Collections.Generic;
using System.Text;

namespace Malt.Reporting
{
	internal interface IRenderFilter
	{
		object Filter (object originalValue);
	}
}
