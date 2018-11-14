using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace TxFlow.WFActivities
{
    internal static class CSharpTypeHelper
    {
        internal static Type GetTypeWithinArgument(Type type)
        {
            if (typeof(System.Activities.Argument).IsAssignableFrom(type))
            {
                if (type.IsGenericType)
                {
                    type = type.GetGenericArguments().Single(); // we expect just one generictype for Arguments!
                }
                else
                {
                    type = typeof(object);
                }
            }
            return type;
        }

        internal static string ToCSharpTypeString(Type type, bool withNamespace = true, bool withGenericArguments = true)
        {
            type = GetTypeWithinArgument(type);

            TypeInfo info = (TypeInfo)type;

            string name = type.Name;
            name = name.IndexOf('`') < 0 ? name : name.Remove(name.IndexOf('`')); // remove generictype-string
            if(withGenericArguments)
            {
                name = name + getGenericTypeDefinition(info.GenericTypeArguments.Union(info.GenericTypeParameters));
            }

            if(withNamespace && !type.IsGenericParameter)
            {
                return type.Namespace + "." + name;
            }

            return name;
        }

        

        private static string getGenericTypeDefinition(IEnumerable<Type> types)
        {
            if (types.Any())
            {
                return "<" + (string.Join(",", types.Select(x => ToCSharpTypeString(x)))) + ">";
            }

            return string.Empty;
        }
    }
}
