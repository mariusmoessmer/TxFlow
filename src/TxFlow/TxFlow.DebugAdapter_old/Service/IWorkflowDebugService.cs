using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace TxFlow.DebugAdapter.Service
{
[ServiceContract]
public interface IWorkflowDebugService
{
    [OperationContract]
    void InitializeWorkflowInstance(Guid workflowInstanceID, string workflowName);

    [OperationContract]
    void ActivityExecuted(Guid workflowInstanceID, string activityID, 
        Dictionary<string, object> variableValues);

    [OperationContract]
    void WorkflowFinished(Guid workflowInstanceID);
}
}
