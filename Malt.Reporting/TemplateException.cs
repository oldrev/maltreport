//作者：李维
//创建时间：2010-08-23

using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.Serialization;

namespace Malt.Reporting
{
    /// <summary>
    /// 模板渲染异常
    /// </summary>
    [Serializable]
    public class TemplateException : Exception
    {
        public TemplateException()
        {
        }

        public TemplateException(string msg)
            : base(msg)
        {
        }

        public TemplateException(string msg, Exception ex)
            : base(msg, ex)
        {
        }


        protected TemplateException(SerializationInfo si, StreamingContext sc)
            : base(si, sc)
        {
        }


    }
}
