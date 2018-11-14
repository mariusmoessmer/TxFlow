using System;
using System.Activities;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace TxFlow.WFActivities
{
    public class ActivityDescriptor
    {
        # region fields

        protected readonly Type _activityType;
        private readonly ArgumentDescriptor[] inArguments;
        private readonly ArgumentDescriptor[] outArguments;

        #endregion

        #region ctor

        public List<Type> ArgumentTypes = new List<Type>();


        public ActivityDescriptor(Type activityType)
        {
            _activityType = activityType;
            var props = _activityType.GetProperties(System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Public).OrderBy(x => x.GetCustomAttribute<DefaultValueAttribute>() != null);
            inArguments = props.Where(x =>
                {
                    var browsable =  x.GetCustomAttribute<BrowsableAttribute>();
                    if(browsable != null)
                    {
                        return browsable.Browsable && !typeof(OutArgument).IsAssignableFrom(x.PropertyType);
                    }
                    return typeof(InArgument).IsAssignableFrom(x.PropertyType);
                }).Distinct(new RemoveDuplicateArguments()).Select(x => new ArgumentDescriptor(x)).ToArray();
            outArguments = props.Where(x => typeof(OutArgument).IsAssignableFrom(x.PropertyType)).Distinct(new RemoveDuplicateArguments()).Select(x => new ArgumentDescriptor(x)).ToArray();

            ArgumentTypes = inArguments.Select(x => x.TypeWithinArgument).Union(outArguments.Select(y => y.TypeWithinArgument)).Distinct().ToList();
        }

        #endregion

        #region methods

        public virtual Activity CreateActivity(Type[] genericTypes, IEnumerable<object> parameterValues, OutArgument outArg = null)
        {

            Type activityType = this._activityType;
            if(genericTypes != null && genericTypes.Length > 0)
            {
                activityType= this._activityType.MakeGenericType(genericTypes);
            }

            var props = activityType.GetProperties(System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Public);

            var activity = (Activity)Activator.CreateInstance(activityType);

            if(outArg != null)
            {
                if (activity is ActivityWithResult)
                {
                    (activity as ActivityWithResult).Result = outArg;
                }else
                {
                    var outArgumentProps = props.Where(x => typeof(OutArgument).IsAssignableFrom(x.PropertyType)).ToList();
                    if (outArgumentProps.Count() == 1)
                    {
                        outArgumentProps.First().SetValue(activity, outArg);
                        //var propertyType = outArgumentProp.PropertyType;

                        ////propertyType = prepareGenericType(outArgumentProp, outArg, propertyType);

                        //var inarg = (OutArgument)Activator.CreateInstance(propertyType);
                        //inarg.Expression = outArg;

                        //prop.SetValue(activity, inarg);

                    }
                    else
                    {
                        throw new Exception("Activity is not activitywithresult and does not provide exactly one outargument");
                    }
                }
            }


            


            Queue<object> lst = new Queue<object>(parameterValues);

            foreach (var prop in props)
            {
                if (typeof(InArgument).IsAssignableFrom(prop.PropertyType))
                {
                    if (lst.Any())
                    {
                        ActivityWithResult arg = (ActivityWithResult)lst.Dequeue();

                        var propertyType = prop.PropertyType;

                        propertyType = prepareGenericType(prop, arg, propertyType);

                        var inarg = (InArgument)Activator.CreateInstance(propertyType);
                        inarg.Expression = arg;
                        if(inarg.Expression != arg)
                        {
                            throw new Exception("TxFlow: InArgument.Expression has internally changed expression-object - " +
                                "this happens when type of InArgument and type of parameter do not match - " +
                                "maybe order of arguments have changed in generated activity-methods?");
                        }

                        prop.SetValue(activity, inarg);
                    }
                }
            }

            return activity;
        }

        public bool HasReturnValue()
        {
            return this.outArguments.Count() == 1;
        }

        private static Type prepareGenericType(PropertyInfo prop, ActivityWithResult arg, Type propertyType)
        {
            if (propertyType.IsGenericTypeDefinition)
            {

                var t = (TypeInfo)prop.PropertyType;

                propertyType = propertyType.MakeGenericType(arg.ResultType);
            }

            return propertyType;
        }

        //public string ToCSharpMethodCall()
        //{
        //    var props = _activityType.GetProperties(System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Public).OrderBy(x => x.Name);
        //    var inArguments = props.Where(x => typeof(InArgument).IsAssignableFrom(x.PropertyType)).Distinct(new RemoveDuplicateArguments()).Select(x => new ArgumentDescriptor(x)).ToArray();
        //    var outArguments = props.Where(x => typeof(OutArgument).IsAssignableFrom(x.PropertyType)).Distinct(new RemoveDuplicateArguments()).Select(x => new ArgumentDescriptor(x)).ToArray();
        //    string argumentAsString = getArgumentsAsString(inArguments, outArguments, false);


        //    string returnTypeText = outArguments.Count() == 1 ? "return " : "";



        //    return returnTypeText + CSharpTypeHelper.ToCSharpTypeString(this._activityType, false) + " (" + argumentAsString + ")";
        //}

        public string ActivityTypeName
        {
            get
            {
                return CSharpTypeHelper.ToCSharpTypeString(this._activityType, false, false);
            }
        }


        private class RemoveDuplicateArguments : IEqualityComparer<PropertyInfo>
        {
            public bool Equals(PropertyInfo x, PropertyInfo y)
            {
                return x.Name == y.Name;
            }

            public int GetHashCode(PropertyInfo obj)
            {
                return obj.Name.GetHashCode();
            }
        }

        public string ReturnTypeText
        {
            get
            {
                var props = _activityType.GetProperties(System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Public).OrderBy(x => x.Name);
                var outArguments = props.Where(x => typeof(OutArgument).IsAssignableFrom(x.PropertyType)).Distinct(new RemoveDuplicateArguments()).Select(x => new ArgumentDescriptor(x)).ToArray();
                return outArguments.Count() == 1 ? outArguments.First().CSharpTypeString : "void";
            }
        }

        public string ActivityName
        {
            get
            {
                return CSharpTypeHelper.ToCSharpTypeString(this._activityType, false);
            }
        }

        public string ActivityNameWithoutGenerics
        {
            get
            {
                return CSharpTypeHelper.ToCSharpTypeString(this._activityType, false, false);
            }
        }

        public Assembly ActivityAssembly { get
            {
                return this._activityType.Assembly;
            }
        }

        public string GetArgumentsAsString(bool withType = true)
        {
            return string.Join(", ", inArguments.Select(y => y.GetTypeStringAndNameAndDefaultValue(withType)).Union(outArguments.Count() == 1 ? new string[0] : outArguments.Select(y => "out " + (withType ? y.TypeStringAndName : y.Name))));
        }

        public IEnumerable<ArgumentDescriptor> GetOutArgumentNames()
        {
            return this.outArguments;
        }

        public string GetArgumentsAsDictionaryKeyValuePair()
        {
            return string.Join(", ", inArguments.Select(y => "{ nameof(" + y.Name + "), " + y.Name + "}").Union(outArguments.Count() == 1 ? new string[0] : outArguments.Select(y => "{ \"" + y.Name + "\", " + y.Name + "}")));
        }

        public virtual string ToCSharpMethodSignature()
        {
            string argumentAsString = GetArgumentsAsString();
            return toChSharpMethodSignature(argumentAsString);
        }

        private string toChSharpMethodSignature(string argumentAsString)
        {
            return ReturnTypeText + " " + CSharpTypeHelper.ToCSharpTypeString(this._activityType, false) + " (" + argumentAsString + ")";
        }

        public bool HasOutArguments()
        {
            return this.outArguments.Any();
        }

        //public virtual string ToCSharpMethodWithInArgumentClass()
        //{
        //    string argumentAsString = string.Join(", ", new[] { ParamsClassName + " param"}.Union(outArguments.Count() == 1 ? new string[0] : outArguments.Select(y => "out " + y.TypeStringAndName)));
        //    return "public virtual " + toChSharpMethodSignature(argumentAsString) + Environment.NewLine + 
        //           "{" + Environment.NewLine +
        //           "    " + CSharpTypeHelper.ToCSharpTypeString(this._activityType, false) + "(" + this.inArguments.Select();
        //}

        //public string ParamsClassName
        //{
        //    get
        //    {
        //        return ActivityName + "Params";
        //    }
        //}

        //public virtual string ToInArgumentClass(string indent)
        //{
        //    return indent+"public class " + ParamsClassName + Environment.NewLine
        //        + indent+"{" + Environment.NewLine
        //        + indent+string.Join(Environment.NewLine + indent, this.inArguments.Select(x => "    public " + x.TypeStringAndName + ";")) + Environment.NewLine
        //        + indent+"}";
        //}


        #endregion
    }
}
