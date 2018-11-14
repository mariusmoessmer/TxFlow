using System;
using System.ComponentModel;
using System.Reflection;

namespace TxFlow.WFActivities
{
    public class ArgumentDescriptor
    {
        #region fields

        private readonly string _name;
        private readonly Type _type;
        private string _defaultValueAsString = null;

        #endregion

        #region ctor

        public ArgumentDescriptor(PropertyInfo x)
        {
            _name = x.Name;
            _type = x.PropertyType;

            var tmp = x.GetCustomAttribute<DefaultValueAttribute>();
            if (tmp != null)
            {
                if (tmp.Value == null)
                {
                    _defaultValueAsString = "null";
                } else if (tmp.Value is bool)
                {
                    _defaultValueAsString = ((bool)tmp.Value) ? "true" : "false";
                } else
                {
                    _defaultValueAsString = tmp.Value.ToString();
                }
            }
        }

        #endregion

        #region properties

        public string Name => _name;

        public Type Type => _type;

        public Type TypeWithinArgument
        {
            get
            {
                return CSharpTypeHelper.GetTypeWithinArgument(this.Type);
            }
        }


        public string TypeStringAndName
        {
            get
            {
                return CSharpTypeHelper.ToCSharpTypeString(this.Type) + " " + this.Name;
            }
        }

        public string CSharpTypeString
        {
            get
            {
                return CSharpTypeHelper.ToCSharpTypeString(this.Type);
            }
        }

        public string GetTypeStringAndNameAndDefaultValue(bool withType)
        {

            //return (withType ? this.TypeStringAndName : this.Name) + (_defaultValueAsString != null ? " = " + _defaultValueAsString : string.Empty);
            
            // Comment MA: defaultvalues not supported for now because there are problems with following out-arguments..
            return (withType ? this.TypeStringAndName : this.Name);
        }

        #endregion
    }
}