using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Fluid;
using Fluid.Ast;
using Fluid.Parser;
using NUnit.Framework;

namespace Sandwych.Reporting.Tests
{
    public class Item
    {
        public int Number { get; set; }
    }

    [TestFixture]
    public class FluidTest
    {

        [Test]
        public async Task FluidShouldWorksFine()
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

            var parser = FluidParserHolder.Instance;
            Assert.True(parser.TryParse(source, out var template));

            var context = new Fluid.TemplateContext();
            context.Options.MemberAccessStrategy.Register(model.GetType()); // Allows any public property of the model to be used
            context.Options.MemberAccessStrategy.Register(typeof(Item));
            context.SetValue("p", model);
            var result = await template.RenderAsync(context);

            Assert.AreEqual($"Hello {model.Str1} {model.Str2} [{model.Numbers[0].Number}{model.Numbers[1].Number}]", result);
        }

        [Test]
        public async Task FluidShouldWorksWithDynamicObject()
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

            var parser = FluidParserHolder.Instance;
            var source = "Hello {{p.Str1}} {{ p.Str2 }} [{% for i in p.Numbers %}{{i.Number}}{% endfor %}]";
            Assert.True(parser.TryParse(source, out var template));
            var context = new Fluid.TemplateContext();
            context.SetValue("p", Fluid.Values.FluidValue.Create(model, new Fluid.TemplateOptions()));
            context.Options.MemberAccessStrategy.Register(model.GetType() as Type);
            context.Options.MemberAccessStrategy.Register(typeof(Item));
            var result = await template.RenderAsync(context);

            Assert.AreEqual($"Hello {model.Str1} {model.Str2} [{model.Numbers[0].Number}{model.Numbers[1].Number}]", result);

        }

        [Test]
        public void TestParseFailedBehaviour()
        {
            var result = FluidParserHolder.Instance.TryParse("{{ 1 ", out var template, out var error);
            Assert.IsFalse(result);
            Assert.IsNull(template);
            TestContext.Out.WriteLine($"Parser error messages: {error}");
        }

        [Test]
        public void TestParserStatements()
        {
            var templateText = "fdsafdsafdsa"; //"{% if 1 > 2 %} 1 {% else %} {{ 2 }} {% endif %}";
            var result = FluidParserHolder.Instance.TryParse(templateText, out var template, out var error);
            Assert.IsTrue(result);
            Assert.NotNull(template);
        }
    }
}
