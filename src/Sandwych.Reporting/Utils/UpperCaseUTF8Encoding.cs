// http://www.distribucon.com/2008/01/29/MakingTheEncodingPropertyUpperCaseDuringXmlSerialization.aspx


using System;
using System.Collections.Generic;
using System.Text;

namespace Sandwych.Reporting.Utils
{
    public class UpperCaseUTF8Encoding : UTF8Encoding
    {
        // Code from a blog http://www.distribucon.com/blog/CategoryView,category,XML.aspx
        //
        // Dan Miser - Thoughts from Dan Miser
        // Tuesday, January 29, 2008 
        // He used the Reflector to understand the heirarchy of the encoding class
        //
        //      Back to Reflector, and I notice that the Encoding.WebName is the property used to
        //      write out the encoding string. I now create a descendant class of UTF8Encoding.
        //      The class is listed below. Now I just call XmlTextWriter, passing in
        //      UpperCaseUTF8Encoding.UpperCaseUTF8 for the Encoding type, and everything works
        //      perfectly. - Dan Miser
        private static readonly object _s_lock = new object();

        public override string WebName =>
            _webName;

        public override string HeaderName => 
            _webName;

        public override string BodyName =>
            _webName;

        public static UpperCaseUTF8Encoding UpperCaseUTF8
        {
            get
            {
                lock (_s_lock)
                {
                    if (_upperCaseUtf8Encoding == null)
                    {
                        _upperCaseUtf8Encoding = new UpperCaseUTF8Encoding();
                        _webName = Encoding.UTF8.WebName.ToUpperInvariant();
                    }
                    return _upperCaseUtf8Encoding;
                }
            }
        }

        private static UpperCaseUTF8Encoding _upperCaseUtf8Encoding = null;
        private static string _webName = null;
    }

}
