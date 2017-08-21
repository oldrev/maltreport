//Creation Time: 2010-08-23
using System;

namespace Sandwych.Reporting
{
    /// <summary>
    /// Exception caused by a Document Template rendering error
    /// </summary>
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
    }
}