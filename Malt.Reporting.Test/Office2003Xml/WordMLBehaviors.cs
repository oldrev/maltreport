using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

using Malt.Reporting.Test;

namespace Malt.Reporting.OfficeXml.Test
{
    internal static class WordMLBehaviors
    {
        static readonly string[] WordML2003SchemaFiles = new string[] 
        {
                @"resources/schemas/word2003/wordnet.xsd",
                @"resources/schemas/word2003/wordnetaux.xsd",
                @"resources/schemas/word2003/wordsp.xsd",
                @"resources/schemas/word2003/office.xsd",
                @"resources/schemas/word2003/vml.xsd",
                @"resources/schemas/word2003/aml.xsd",
                @"resources/schemas/word2003/xsdlib.xsd",
                @"resources/schemas/word2003/w10.xsd",
        };

        public static void ShouldBeWellFormedWordML(this XmlDocument xml)
        {
            xml.ShouldWellFormed(WordML2003SchemaFiles);
        }
    }
}
