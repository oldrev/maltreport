using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.Xml.Linq;

namespace Sandwych.Reporting.Xml
{
    public static class XmlNamespaceManagerExtensions
    {
        public static XNamespace GetNamespace(this XmlNamespaceManager self, string nsPrefix)
        {
            XNamespace ns = self.LookupNamespace(nsPrefix);
            return ns;
        }
    }
}
