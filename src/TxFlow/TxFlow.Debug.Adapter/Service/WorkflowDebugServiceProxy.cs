using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TxFlow.Debug.Adapter.Service
{
    class WorkflowDebugServiceProxy : IWorkflowDebugService
    {
        public static IWorkflowDebugService RealWorkflowService = null;

        void IWorkflowDebugService.ActivityExecuted(string workflowDefinitionID, string workflowInstanceID, string activityID, Dictionary<string, object> variableValues)
        {
            RealWorkflowService?.ActivityExecuted(workflowDefinitionID, workflowInstanceID, activityID, variableValues);
        }
    }
}
