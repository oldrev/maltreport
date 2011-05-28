//作者：李维
//创建时间：2010-09-03

using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;

namespace Malt.Reporting.Xml
{
    internal interface IXmlNodeProcessor
    {
        void ProcessNode(XmlNode node);
    }
}
