//作者：李维
//创建时间：2010-08-20

using System;
using System.Collections.Generic;
using System.Text;

using Bravo.Reporting.OpenDocument;

namespace Bravo.Reporting
{
    public interface ITemplateCompiler
    {

        /// <summary>
        /// 编译模板
        /// </summary>
        /// <param name="inputOdf"></param>
        /// <returns></returns>
        OdfDocument Compile(OdfDocument inputOdf);

    }
}
