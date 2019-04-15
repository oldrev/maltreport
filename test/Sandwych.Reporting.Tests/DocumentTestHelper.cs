using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Reflection;

namespace Sandwych.Reporting.Tests
{

    public static class DocumentTestHelper
    {

        public static Stream GetResource(string name)
        {
            return typeof(DocumentTestHelper).GetTypeInfo().Assembly.GetManifestResourceStream(name);
        }


    }

}
