using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace Sandwych.Reporting
{

    public class DocumentGenerationEngine
    {
        private readonly Configuration _configuration;

        public DocumentGenerationEngine(Configuration configuration)
        {
            _configuration = configuration;
        }



        public ITemplate OpenTemplate<TemplateType>(string templatePath)
            where TemplateType : ITemplate, new()
        {
            var template = Activator.CreateInstance(typeof(TemplateType)) as ITemplate;
            template.Load(templatePath);
            return template;
        }

        public ITemplate OpenTemplate<TemplateType>(Stream templateStream)
            where TemplateType : ITemplate, new()
        {
            var template = Activator.CreateInstance(typeof(TemplateType)) as ITemplate;
            template.Load(templateStream);
            return template;
        }


    }
}
