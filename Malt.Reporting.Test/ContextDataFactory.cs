using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Malt.Reporting.Test.Models;

namespace Malt.Reporting.Test
{
    internal static class ContextDataFactory
    {

        public static IDictionary<string, object> CreateComplexDataContext()
        {
            var ctx = new Dictionary<string, object>()
            {
                { "var1", "Hello" },
                { "var2", "World" },
                { "stringArray", "The quick brown fox jumps over the lazy dog".Split(' ') },
                { "intArray", new int[] {1, 2, 3, 4, 5} },
                { "employees", new Employee[5]  },

                { "image1", "resources/go-home.PNG" },
            };

            return ctx;

        }
    }
}
