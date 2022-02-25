using System;
using System.Collections.Generic;
using System.Globalization;
using Sandwych.Reporting.Textilize;

namespace Sandwych.Reporting
{
    public class TemplateContext
    {
        public TemplateContext(object model, TemplateOptions options = default, bool allowModelMembers = true)
        {
            this.Model = model;
            this.Options = options ?? TemplateOptions.Default;
            this.AllowModelMembers = allowModelMembers;
        }

        private List<Type> _typeMembersAllowList = new List<Type>();

        public IReadOnlyList<Type> TypeMembersAllowList => _typeMembersAllowList;

        public object Model { get;  }

        public bool AllowModelMembers { get; }

        public TemplateOptions Options { get; }

        public TemplateContext AllowMembersAccessTo<TType>()
        {
            _typeMembersAllowList.Add(typeof(TType));
            return this;
        }

        public TemplateContext AllowMembersAccessTo(Type type)
        {
            _typeMembersAllowList.Add(type);
            return this;
        }
    }
}