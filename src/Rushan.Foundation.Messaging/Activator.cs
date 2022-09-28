using System;
using System.Collections.Concurrent;
using System.Linq;

namespace Rushan.Foundation.Messaging
{
    internal static class Activator
    {
        private static readonly ConcurrentDictionary<string, Type> TypeMapCache = new ConcurrentDictionary<string, Type>();

        public static Type GetType(string typeHint)
        {
            if (string.IsNullOrWhiteSpace(typeHint)) return null;

            Type type;
            if (TypeMapCache.TryGetValue(typeHint, out type))
            {
                return type;
            }

            type = AppDomain.CurrentDomain.GetAssemblies()
                .Where(a => !a.IsDynamic)
                .SelectMany(a => a.GetExportedTypes())
                .FirstOrDefault(t => t.FullName.Equals(typeHint));

            TypeMapCache.TryAdd(typeHint, type);

            return type;
        }
    }
}
