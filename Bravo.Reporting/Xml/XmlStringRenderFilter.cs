using System;
using System.Collections.Generic;
using System.Text;
using System.Security;
using System.Diagnostics;

namespace Bravo.Reporting.Xml
{
    internal class XmlStringRenderFilter : IRenderFilter
    {
        #region IRenderFilter 成员

        public object Filter(object originalValue)
        {
            var originalStr = originalValue as string;
            Debug.Assert(originalStr != null);
            return SecurityElement.Escape(originalStr);
        }

        #endregion
    }
}
