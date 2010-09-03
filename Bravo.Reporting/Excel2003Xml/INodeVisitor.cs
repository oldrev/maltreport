//作者：李维
//创建时间：2010-09-03

using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;

namespace Bravo.Reporting.Excel2003Xml
{
    internal interface INodeVisitor
    {
        void ProcessNode(XmlNode node);
    }
}
