using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.Xml;
using Fluid;

namespace Sandwych.Reporting
{
    public abstract class AbstractXmlTemplate<TDocument> : AbstractTemplate<TDocument>
        where TDocument : AbstractXmlDocument<TDocument>, new()
    {
        protected FluidTemplate TextTemplate { get; private set; }

        public AbstractXmlTemplate(TDocument document) : base(document)
        {
            this.PrepareTemplate();

            var stringTemplate = this.GetStringTemplate();
            if (!FluidTemplate.TryParse(stringTemplate, out var fluidTemplate, out var errors))
            {
                throw new SyntaxErrorException(errors.Aggregate((x, y) => x + "\n" + y));
            }
            this.TextTemplate = fluidTemplate;
        }

        private string GetStringTemplate()
        {
            var sb = new StringBuilder();
            using (var writer = XmlWriter.Create(sb))
            {
                this.TemplateDocument.XmlDocument.WriteTo(writer);
            }
            return sb.ToString();
        }

    }
}
