using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Description;
using TxFlow.Debug.Adapter.DebugInformation;
using TxFlow.Debug.Adapter.Other;

namespace TxFlow.Debug.Adapter.Service
{
    class WorkflowDebugService : IWorkflowDebugService
    {
        public event EventHandler<GenericEventArgs<WorkflowInstanceProxy>> WorkflowInstanceStopped;

        private void onWorkflowInstanceStopped(WorkflowInstanceProxy proxy)
        {
            WorkflowInstanceStopped?.Invoke(this, new GenericEventArgs<WorkflowInstanceProxy>(proxy));
        }



        private object _workflowInstanceDictLocker = new object();

        private Dictionary<int, WorkflowInstanceProxy> _workflowInstancesPerMappedThreadId = new Dictionary<int, WorkflowInstanceProxy>();


        private bool _globalPauseOnNextLine;
        private ServiceHost _wcfServiceHost = null;

        internal void HostService(Uri hostAddress)
        {
            hostWCFService(hostAddress);

            //new System.Threading.Thread(() =>
            //{
            //    WorkflowInstanceProxy proxy = getOrCreateWorkflowInstanceProxy(6);

            //    System.Threading.Thread.Sleep(1000);
            //    foreach(var activityId in DebugInformationProvider.Instance.AllActivityIds)
            //    {
            //        System.Threading.Thread.Sleep(1000);

            //        if (_pauseOnNextLine || DebugInformationProvider.Instance.ActivityHasBreakpoint(activityId)
            //            || proxy.StopOnNextLine)
            //        {

            //            _pauseOnNextLine = false;
            //            proxy.StopOnNextLine = false;
            //            proxy.OnActivityExecuted(activityId);
            //            this._onActivityExecutedAction(proxy);
            //            proxy.AwaitContinuation();
            //        }

            //    }
            //})
            //{
            //    IsBackground = true
            //}.Start();
        }

        private void hostWCFService(Uri uri)
        {
            WorkflowDebugServiceProxy.RealWorkflowService = this;
            // Create the ServiceHost.
            _wcfServiceHost = new ServiceHost(typeof(WorkflowDebugServiceProxy), uri);


            //Add Endpoint to Host
            _wcfServiceHost.AddServiceEndpoint(typeof(IWorkflowDebugService), new WSHttpBinding(), "");

            // Enable metadata publishing.
            ServiceMetadataBehavior smb = new ServiceMetadataBehavior();
            smb.HttpGetEnabled = true;
            smb.MetadataExporter.PolicyVersion = PolicyVersion.Policy15;
            _wcfServiceHost.Description.Behaviors.Add(smb);

            // Open the ServiceHost to start listening for messages
            _wcfServiceHost.Open();
        }

        internal void Disconnect()
        {
            var wcfServiceHost = _wcfServiceHost;
            if (wcfServiceHost != null)
            {
                // Close the ServiceHost.
                wcfServiceHost.Close();
            }
        }

        internal WorkflowInstanceProxy GetWorkflowInstanceProxy(int workflowInstanceId, bool throwException = true)
        {
            lock(_workflowInstanceDictLocker)
            {
                try
                {
                    return this._workflowInstancesPerMappedThreadId[workflowInstanceId];
                }catch(System.Collections.Generic.KeyNotFoundException)
                {
                    if(throwException)
                    {
                        throw;
                    }

                    return null;
                }
            }
        }

        internal IEnumerable<Microsoft.VisualStudio.Shared.VSCodeDebugProtocol.Messages.Thread> GetAllRunningWorkflowsAsThreads()
        {
            lock(_workflowInstanceDictLocker)
            {
                return this._workflowInstancesPerMappedThreadId.Select(x => new Microsoft.VisualStudio.Shared.VSCodeDebugProtocol.Messages.Thread(x.Key, x.Value.WorkflowDebugInformation.WorkflowName));
            }
        }

        internal void Pause()
        {
            _globalPauseOnNextLine = true;
        }

        void IWorkflowDebugService.ActivityExecuted(string workflowDefinitionID, string workflowInstanceID, string activityID, Dictionary<string, object> variableValues)
        {

            int threadId = mapThreadId(workflowInstanceID);

            WorkflowInstanceProxy proxy = null;
            lock (_workflowInstanceDictLocker)
            {
                if (!_workflowInstancesPerMappedThreadId.ContainsKey(threadId))
                {
                    var workflowDebugInformation = DebugInformationProvider.Instance.TryGetDebugInformation(workflowDefinitionID);
                    if (workflowDebugInformation == null)
                    {
                        System.Diagnostics.Debug.WriteLine("No debug-information for workflow with workflowDefinitionID '" + workflowDefinitionID + "'");
                        return;
                    }
                    else
                    {
                        proxy = new WorkflowInstanceProxy(threadId, workflowDebugInformation);
                        _workflowInstancesPerMappedThreadId[threadId] = proxy;
                    }
                }else
                {
                    proxy = _workflowInstancesPerMappedThreadId[threadId];
                }
            }

            if(activityID == "-1")
            {
                // whole workflow completed - mark it as completed
                _workflowInstancesPerMappedThreadId.Remove(threadId);
            }else
            {
                bool pause = proxy.OnActivityExecuted(activityID, variableValues);

                if (_globalPauseOnNextLine || pause)
                {
                    _globalPauseOnNextLine = false;
                    proxy.StopOnNextLine = false;
                    this.onWorkflowInstanceStopped(proxy);
                    proxy.WaitUntilUserContinues();
                }
            }
        }

        private int mapThreadId(string workflowInstanceID)
        {
            // just return hashcode for sake of simplicity 
            return workflowInstanceID.GetHashCode();
        }
    }
}
