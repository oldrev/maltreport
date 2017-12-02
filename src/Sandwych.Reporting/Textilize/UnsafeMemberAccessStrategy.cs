using Fluid;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace Sandwych.Reporting.Textilize
{
    public class UnsafeMemberAccessStrategy : IMemberAccessStrategy
    {
        private readonly Dictionary<string, IMemberAccessor> _map = new Dictionary<string, IMemberAccessor>();
        private readonly IMemberAccessStrategy _parent;

        public UnsafeMemberAccessStrategy(IMemberAccessStrategy parent)
        {
            _parent = parent;
        }

        public IMemberAccessor GetAccessor(object obj, string name)
        {
            var type = obj.GetType();

            if (_map.Count > 0)
            {
                while (type != null)
                {
                    // Look for specific property map
                    if (_map.TryGetValue(Key(type, name), out var accessor))
                    {
                        return accessor;
                    }

                    // Look for a catch-all getter
                    if (_map.TryGetValue(Key(type, "*"), out accessor))
                    {
                        return accessor;
                    }

                    type = type.GetTypeInfo().BaseType;
                }
            }

            var parentAccessor = _parent?.GetAccessor(obj, name);

            if (parentAccessor != null)
            {
                return parentAccessor;
            }

            //Register the object type and try again
            this.Register(obj.GetType());

            return this.GetAccessor(obj, name);
        }

        public void Register(Type type, string name, IMemberAccessor getter)
        {
            _map[Key(type, name)] = getter;
        }

        private string Key(Type type, string name) => $"{type.Name}.{name}";
    }
}