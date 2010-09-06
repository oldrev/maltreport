//作者：李维
//创建时间：2010-09-03

using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;

namespace Bravo.Reporting.Xml
{
    internal interface IXmlNodeVisitor
    {
        void ProcessNode(XmlNode node);
    }
}
