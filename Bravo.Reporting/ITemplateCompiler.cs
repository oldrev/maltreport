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
