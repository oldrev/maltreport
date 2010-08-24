//作者：李维
//创建时间：2010-08-20

using System;
using System.Collections.Generic;
using System.Text;

namespace Bravo.Reporting
{
    public interface ITemplateCompiler
    {

        OdfArchive Compile(OdfArchive inputOdf);

    }
}
