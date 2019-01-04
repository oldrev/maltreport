/**
 * This class is copy from: https://github.com/sebastienros/fluid/issues/11#issuecomment-351401013
 * A big slute to @pfeigl
 * */
using Fluid;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace Sandwych.Reporting.Textilize
{
    public class UnsafeMemberAccessStrategy : IMemberAccessStrategy
    {
        private readonly MemberAccessStrategy baseMemberAccessStrategy = new MemberAccessStrategy();

        public IMemberAccessor GetAccessor(Type type, string name)
        {
            var accessor = baseMemberAccessStrategy.GetAccessor(type, name);
            if (accessor != null)
            {
                return accessor;
            }

            baseMemberAccessStrategy.Register(type);
            return baseMemberAccessStrategy.GetAccessor(type, name);
        }

        public void Register(Type type, string name, IMemberAccessor getter)
        {
            baseMemberAccessStrategy.Register(type, name, getter);
        }
    }
}