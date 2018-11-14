using System;
using System.Activities;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TxFlow.WFActivities
{
    public class CreateActivityParams
    {
        private string activityName;
        private IEnumerable<object> parameterValues;
        private OutArgument outArg = null;

        public string ActivityName { get => activityName; set => activityName = value; }
        public IEnumerable<object> ParameterValues { get => parameterValues; set => parameterValues = value; }
        public OutArgument OutArg { get => outArg; set => outArg = value; }
        public Type[] GenericTypes { get; set; }
    }
}
