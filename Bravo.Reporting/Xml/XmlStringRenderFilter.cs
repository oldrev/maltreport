using System;
using System.Collections.Generic;
using System.Text;
using System.Security;
using System.Diagnostics;

namespace Bravo.Reporting.Xml
{
    internal sealed class XmlStringRenderFilter : IRenderFilter
    {
        #region IRenderFilter Members

        public object Filter(object originalValue)
        {
            Debug.Assert(originalValue is string);
            var originalStr = originalValue as string;
            return SecurityElement.Escape(originalStr);
        }

        #endregion
    }
}
