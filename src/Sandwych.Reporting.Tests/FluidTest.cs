using System;
using System.Collections.Generic;
using System.Text;
using Fluid;
using Xunit;

namespace Sandwych.Reporting.Tests
{
    public class Item
    {
        public int Number { get; set; }
    }

    public class FluidTest
    {

        [Fact]
        public void FluidShouldWorksFine()
        {
            var model = new
            {
                Str1 = "Fluid",
                Str2 = "Template",
                Numbers = new Item[]
                {
                    new Item { Number = 1 },
                    new Item { Number = 2 }
                }
            };
            var source = "Hello {{p.Str1}} {{ p.Str2 }} [{% for i in p.Numbers %}{{i.Number}}{% endfor %}]";

            Assert.True(FluidTemplate.TryParse(source, out var template));

            var context = new TemplateContext();
            context.MemberAccessStrategy.Register(model.GetType()); // Allows any public property of the model to be used
            context.MemberAccessStrategy.Register(typeof(Item));
            context.SetValue("p", model);
            var result = template.Render(context);

            Assert.Equal($"Hello {model.Str1} {model.Str2} [{model.Numbers[0].Number}{model.Numbers[1].Number}]", result);
        }

        [Fact(DisplayName ="Fluid with Dynamic Object")]
        public void FluidShouldWorksWithDynamicObject()
        {
            dynamic model = new
            {
                Str1 = "Fluid",
                Str2 = "Template",
                Numbers = new Item[]
                {
                    new Item { Number = 1 },
                    new Item { Number = 2 }
                }
            };

            var source = "Hello {{p.Str1}} {{ p.Str2 }} [{% for i in p.Numbers %}{{i.Number}}{% endfor %}]";
            Assert.True(FluidTemplate.TryParse(source, out var template));
            var context = new TemplateContext();
            System.Diagnostics.Debug.WriteLine("{0}", model.Numbers.GetType().ToString() as string);
            context.SetValue("p", model);
            var result = template.Render(context);

            Assert.Equal($"Hello {model.Str1} {model.Str2} [{model.Numbers[0].Number}{model.Numbers[1].Number}]", result);

        }


    }
}
