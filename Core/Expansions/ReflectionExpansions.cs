using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Reflection;

namespace Core.Expansions
{
    public static class ReflectionExpansions
    {
        public static Type[] GetAllTypes(this Type type)
        {
            var res = AppDomain.CurrentDomain
                .GetAssemblies()
                .SelectMany(a => a.GetTypes())
                .Where(x => x.GetInterfaces().Any(y => y == type)
                    || x.BaseType == type)
                .ToArray();

            return res;
        }       
    }
}
