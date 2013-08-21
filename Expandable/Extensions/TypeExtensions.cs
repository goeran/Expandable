using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Expandable.Extensions
{
    public static class TypeExtensions
    {
        public static IEnumerable<ConstructorInfo> PublicConstructors(this Type aClass)
        {
            return aClass.GetConstructors(BindingFlags.Instance | BindingFlags.Public);
        }

        public static bool HasAParameterlessPublicCtor(this Type aClass)
        {
            var constructors = aClass.PublicConstructors();
            return constructors.Any(c => !c.GetParameters().Any());
        }
    }
}
