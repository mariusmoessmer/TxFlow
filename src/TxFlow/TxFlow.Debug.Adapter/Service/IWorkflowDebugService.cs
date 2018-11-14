using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace TxFlow.Debug.Adapter.Service
{
[ServiceContract]
public interface IWorkflowDebugService
{
    [OperationContract]
    void ActivityExecuted(string workflowDefinitionID, string workflowInstanceID, 
        string activityID, Dictionary<string, object> variableValues);
}
}
