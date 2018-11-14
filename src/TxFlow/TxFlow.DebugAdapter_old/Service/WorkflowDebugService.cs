using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Description;
using TxFlow.DebugAdapter.DebugInformation;
using TxFlow.DebugAdapter.Other;

namespace TxFlow.DebugAdapter.Service
{
    class WorkflowDebugService : IWorkflowDebugService
    {
        public event EventHandler<GenericEventArgs<WorkflowInstanceProxy>> WorkflowInstanceStopped;

        private void onWorkflowInstanceStopped(WorkflowInstanceProxy proxy)
        {
            WorkflowInstanceStopped?.Invoke(this, new GenericEventArgs<WorkflowInstanceProxy>(proxy));
        }



        private object _workflowInstanceDictLocker = new object();

        private Dictionary<int, WorkflowInstanceProxy> _workflowInstancesPerThreadId = new Dictionary<int, WorkflowInstanceProxy>();
        private Dictionary<Guid, WorkflowInstanceProxy> _workflowInstancesPerWorkflowInstanceId = new Dictionary<Guid, WorkflowInstanceProxy>();


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

        internal WorkflowInstanceProxy GetWorkflowInstanceProxy(int workflowInstanceId)
        {
            lock(_workflowInstanceDictLocker)
            {
                return this._workflowInstancesPerThreadId[workflowInstanceId];
            }
        }

        internal IEnumerable<Protocol.Thread> GetAllRunningWorkflowsAsThreads()
        {
            lock(_workflowInstanceDictLocker)
            {
                return this._workflowInstancesPerThreadId.Select(x => new Protocol.Thread(x.Key, x.Value.WorkflowDebugInformation.WorkflowName));
            }
        }

        internal void Pause()
        {
            _globalPauseOnNextLine = true;
        }


        private int _nextWorkflowThreadId = 1;
        void IWorkflowDebugService.InitializeWorkflowInstance(Guid workflowInstanceID, string workflowName)
        {
            lock(_workflowInstanceDictLocker)
            {
                var workflowDebugInformation = DebugInformationProvider.Instance.TryGetDebugInformation(workflowName);
                if(workflowDebugInformation == null)
                {
                    System.Diagnostics.Debug.WriteLine("No debug-information for workflow with name '"+workflowName+"'");
                }
                else
                {
                    int workflowThreadID = _nextWorkflowThreadId++;
                    var proxy = new WorkflowInstanceProxy(workflowInstanceID, workflowThreadID, workflowDebugInformation);
                    _workflowInstancesPerWorkflowInstanceId[workflowInstanceID] = proxy;
                    _workflowInstancesPerThreadId[workflowThreadID] = proxy;
                }
            }
        }

        void IWorkflowDebugService.ActivityExecuted(Guid workflowInstanceID, string activityID, Dictionary<string, object> variableValues)
        {
            WorkflowInstanceProxy proxy = null;
            lock (_workflowInstanceDictLocker)
            {
                if (!_workflowInstancesPerWorkflowInstanceId.ContainsKey(workflowInstanceID))
                {
                    return; // do not care about workflows that were not initialized correctly
                }

                proxy = _workflowInstancesPerWorkflowInstanceId[workflowInstanceID];

            }

            bool pause = proxy.OnActivityExecuted(activityID, variableValues);

            if (_globalPauseOnNextLine || pause)
            {
                _globalPauseOnNextLine = false;
                proxy.StopOnNextLine = false;
                this.onWorkflowInstanceStopped(proxy);
                proxy.WaitUntilUserContinues();
            }
        }

        void IWorkflowDebugService.WorkflowFinished(Guid workflowInstanceID)
        {
            lock (_workflowInstanceDictLocker)
            {
                if (!_workflowInstancesPerWorkflowInstanceId.ContainsKey(workflowInstanceID))
                {
                    return; // do not care about workflows that were not initialized correctly
                }

                var proxy = _workflowInstancesPerWorkflowInstanceId[workflowInstanceID];
                _workflowInstancesPerWorkflowInstanceId.Remove(workflowInstanceID);
                _workflowInstancesPerThreadId.Remove(proxy.ThreadId);
            }
        }
    }
}
