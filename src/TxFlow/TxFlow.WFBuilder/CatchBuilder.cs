using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TxFlow.WFBuilder
{
    public class CatchBuilder
    {
        public CatchBuilder(string typeFullName, string exceptionVariableName, Action<WorkflowPath> workflowPathAction)
        {
            TypeFullName = typeFullName;
            ExceptionVariableName = exceptionVariableName;
            WorkflowPathAction = workflowPathAction;
        }

        public string TypeFullName { get; internal set; }
        public string ExceptionVariableName { get; internal set; }
        public Action<WorkflowPath> WorkflowPathAction { get; set; }
    }
}
