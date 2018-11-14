using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TxFlow.DebugAdapter.Service
{
    class WorkflowDebugServiceProxy : IWorkflowDebugService
    {
        public static IWorkflowDebugService RealWorkflowService = null;

        void IWorkflowDebugService.ActivityExecuted(Guid workflowInstanceID, string activityID, Dictionary<string, object> variableValues)
        {
            RealWorkflowService?.ActivityExecuted(workflowInstanceID, activityID, variableValues);
        }

        void IWorkflowDebugService.InitializeWorkflowInstance(Guid workflowInstanceID, string workflowName)
        {
            RealWorkflowService?.InitializeWorkflowInstance(workflowInstanceID, workflowName);
        }

        void IWorkflowDebugService.WorkflowFinished(Guid workflowInstanceID)
        {
            RealWorkflowService?.WorkflowFinished(workflowInstanceID);
        }
    }
}
