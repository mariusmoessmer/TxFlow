using System;
using System.Linq;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;

namespace TxFlow.CSharpDSL
{
    public class ActivityInvocationLog
    {
        private readonly List<ActivityInvocation> _activityInvocations = new List<ActivityInvocation>();

        public ActivityInvocation this[int idx]
        {
            get
            {
                return _activityInvocations[idx];
            }
        }

        public IEnumerable<ActivityInvocation> ActivityInvocations
        {
            get
            {
                return this._activityInvocations;
            }
        }

        public void Log(ActivityInvocation activityInvocation)
        {
            this._activityInvocations.Add(activityInvocation);
        }

        private static string objectToStringForValueForViewing(object value)
        {
            return value == null ? "{{NULL}}" : value.ToString();
        }

        public string WriteAsJson(string filePath)
        {
            var activityNameAndParams = ActivityInvocations.Select(x =>
            {
                IDictionary<string, object> activities = new SortedDictionary<string, object>(x.ParameterValuesByName);
                if(!object.Equals(typeof(void), x.ReturnValue))
                {
                    activities["return"] = x.ReturnValue;
                }

                var result = new Dictionary<string, IDictionary<string, object>>();
                result[x.ActivityName] = activities;
                return result;
            }).ToArray();

            string json = JsonConvert.SerializeObject(activityNameAndParams, Formatting.Indented);
            File.WriteAllText(filePath, json);

            return filePath;
        }
    }

}
