using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;

using Antlr3.ST;
using Antlr3.ST.Language;


namespace Bravo.Reporting
{
    internal class SafeStringRenderer : IAttributeRenderer
    {
        #region IAttributeRenderer 成员

        public string ToString(object o)
        {
            Debug.Assert(o is string);
            var rawStr = o as string;
            return System.Security.SecurityElement.Escape(rawStr);
        }

        public string ToString(object o, string formatName)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
