using System;
using System.Activities;
using System.Activities.Expressions;
using System.Activities.Statements;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;

namespace TxFlow.WFActivities
{
    public class WFActivityTypeHelper
    {       
        public static Activity CreateActivity(CreateActivityParams p)
        {
            var activityType = AppDomain.CurrentDomain.GetAssemblies().SelectMany(x => x.GetTypes())
                .FirstOrDefault(x => typeNameWithoutGenerics(x.Name) == p.ActivityName);
            if (activityType == null)
            {
                throw new Exception($"Cannot resolve activityType {p.ActivityName}");
            }

            var activityDescriptor = new ActivityDescriptor(activityType);

            //var activityDescriptor = getActivities().Single(x => x.ActivityTypeName == p.ActivityName);

            return activityDescriptor.CreateActivity(p.GenericTypes, p.ParameterValues, p.OutArg);
        }

        private static string typeNameWithoutGenerics(string name)
        {
            //Name    "GetMetaDataValueActivity`1"    string

            int idx = name.IndexOf("`");
            if(idx >= 0)
            {
                return name.Substring(0, idx);
            }

            return name;
        }

        public static ActivityWithResult CreateArgumentConstantValue(object value, string valueTypeName)
        {
            var type = WFActivities.WFActivityTypeHelper.ResolveType(valueTypeName);
            var literal = typeof(Literal<>).MakeGenericType(type);


            object valueWithCorrectType = value;
            if (type.IsEnum)
            {
                valueWithCorrectType = Enum.ToObject(type, value);
            }

            return (ActivityWithResult)Activator.CreateInstance(literal, valueWithCorrectType);
        }

        public static ActivityWithResult CreateArgumentExpression(string expression, string expressionResultTypeName)
        {
            var type = WFActivities.WFActivityTypeHelper.ResolveType(expressionResultTypeName);
            var csharpValueType = typeof(Microsoft.CSharp.Activities.CSharpValue<>).MakeGenericType(type);
            return (ActivityWithResult)Activator.CreateInstance(csharpValueType, expression);
        }


        private static IEnumerable<ActivityDescriptor> _cachedActivities = null;

        private static IEnumerable<ActivityDescriptor> getActivities()
        {
            return _cachedActivities ?? (_cachedActivities = createActivities());
        }

        private static IEnumerable<ActivityDescriptor> createActivities()
        {
            return new ActivityDescriptor[] { };
            //var tmp = typeof(HolidayApproval.Entities.HolidayRequestEntity); // make sure this is loaded

            //var hallo = new HolidayApproval.Entities.HolidayRequestEntity();
            //var test = Type.GetType(tmp.FullName);

            //yield return new ActivityDescriptor(typeof(HolidayApproval.Activities.BookHolidayRequestActivity));
            //yield return new ActivityDescriptor(typeof(HolidayApproval.Activities.CreateHolidayApprovalTaskActivity));
            //yield return new ActivityDescriptor(typeof(HolidayApproval.Activities.SendEmailActivity));
            //yield return new ActivityDescriptor(typeof(HolidayApproval.Activities.UpdateHolidayRequestStateActivity));

            //foreach (var cat in InfPro.Dotiga.ActivityProvider.ActivityProviderFacade.Instance.GetToolboxCategories())
            //{
            //    foreach (var tool in cat.Tools)
            //    {
            //        Assembly toolAssembly = AppDomain.CurrentDomain.GetAssemblies().FirstOrDefault(assembly => assembly.GetName().Name == tool.AssemblyName);

            //        if(toolAssembly == null)
            //        {
            //            toolAssembly = Assembly.Load(tool.AssemblyName);
            //        }

            //        Type discoveredToolType = toolAssembly.GetType(tool.ToolName, true, true);

            //        yield return new ActivityDescriptor(discoveredToolType);
            //    }
            //}
        }

        public static Type ResolveType(string fullTypeName)
        {
            string withoutGenerics = typeNameWithoutGenerics(fullTypeName);

            IEnumerable<Type> types =
                    from a in AppDomain.CurrentDomain.GetAssemblies()
                    from t in a.GetTypes()
                    select t;

            var type = types.FirstOrDefault(x => typeNameWithoutGenerics(x.FullName) == withoutGenerics);
            if(type == null)
            {
                throw new Exception($"Cannot resolve type {fullTypeName}");
            }
            

            if(fullTypeName != withoutGenerics)
            {
                // ok we have generics...
                var regexMatch = Regex.Match(fullTypeName, @".*\[\[(?<genericTypes>.*)\]\]"); // TODO: think about recursive generic types
                string genericTypes = regexMatch.Groups["genericTypes"].Value;
                Type[] genericTypeArray = genericTypes.Split(',').Select(x => ResolveType(x.Trim())).ToArray();

                return type.MakeGenericType(genericTypeArray);
            }


            return type;
        }

        
    }
}
