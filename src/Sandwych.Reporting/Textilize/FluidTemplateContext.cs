using System;
using System.Collections.Generic;
using System.Text;
using Fluid;

namespace Sandwych.Reporting.Textilize
{
    public class FluidTemplateContext : TemplateContext
    {
        public const string OutputDocumentName = "__document";

        public IDocument OutputDocument => this.AmbientValues[OutputDocumentName] as IDocument;

        public FluidTemplateContext(IDocument outputDocument, IReadOnlyDictionary<string, object> context)
        {
            this.MemberAccessStrategy = new UnsafeMemberAccessStrategy(GlobalMemberAccessStrategy);

            this.AmbientValues[OutputDocumentName] = outputDocument;

            foreach (var pair in context)
            {
                this.SetValue(pair.Key, Fluid.Values.FluidValue.Create(pair.Value));
                if (pair.Value != null)
                {
                    this.MemberAccessStrategy.Register(pair.Value.GetType());
                }
            }
        }
    }
}
