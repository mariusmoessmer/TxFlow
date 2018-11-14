using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace TxFlow.CSharpDSL
{
    public class ActivityInvocation
    {
        public ActivityInvocation(string activityName, Dictionary<string, object> parameterValuesByName, object returnValue)
        {
            this.ActivityName = activityName;
            this.ParameterValuesByName = parameterValuesByName;
            this.ParameterValues = parameterValuesByName.Values.ToArray();
            this.ReturnValue = returnValue;

        }

        private object cloneArgumentValueObject(object value)
        {
            if (value == null)
            {
                return null;
            }
            NetDataContractSerializer serializer = new NetDataContractSerializer(); // WF4 uses this serializer
            using (MemoryStream ms = new MemoryStream())
            {
                serializer.Serialize(ms, value);
                ms.Position = 0;
                return serializer.Deserialize(ms);
            }
        }

        public string ActivityName { get; }
        public object[] ParameterValues { get; }
        public object ReturnValue { get; }
        public Dictionary<string, object> ParameterValuesByName { get; }
    }
}
