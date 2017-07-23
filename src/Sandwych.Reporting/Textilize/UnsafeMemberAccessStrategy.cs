using Fluid;
using System;
using System.Collections.Generic;

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

        public object Get(object obj, string name)
        {
            // Look for specific property map
            if (_map.TryGetValue(Key(obj.GetType(), name), out var getter))
            {
                return getter.Get(obj, name);
            }

            // Look for a catch-all getter
            if (_map.TryGetValue(Key(obj.GetType(), "*"), out getter))
            {
                return getter.Get(obj, name);
            }

            var parentAccessor = _parent?.Get(obj, name);
            if (parentAccessor != null)
            {
                return parentAccessor;
            }

            //Register the object type and try again
            this.Register(obj.GetType());

            return this.Get(obj, name);
        }

        public void Register(Type type, string name, IMemberAccessor getter)
        {
            _map[Key(type, name)] = getter;
        }

        private string Key(Type type, string name) => $"{type.Name}.{name}";
    }
}