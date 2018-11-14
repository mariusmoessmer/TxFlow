using Microsoft.VisualStudio.Shared.VSCodeDebugProtocol.Messages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using TxFlow.Debug.Adapter.DebugInformation;

namespace TxFlow.Debug.Adapter
{
    class WorkflowInstanceProxy
    {
        public WorkflowInstanceProxy(int threadId, WorkflowDebugInformation workflowDebugInformation)
        {
            this.ThreadId = threadId;
            this.WorkflowDebugInformation = workflowDebugInformation;
        }

        private bool _debuggeeExecuting = false;
        private System.Threading.AutoResetEvent _waitUntilWorkflowStopsResetEvent = new System.Threading.AutoResetEvent(false);

        public void WaitForSuspend()
        {
            if (_debuggeeExecuting)
            {
                _waitUntilWorkflowStopsResetEvent.WaitOne();
                _debuggeeExecuting = false;
            }
        }
        public bool IsRunning
        {
            get
            {
                return false;
            }
        }

        public int ThreadId { get; internal set; }

        public Guid WorkflowInstanceId { get; internal set; }

        public WorkflowDebugInformation WorkflowDebugInformation { get; private set; }



        private bool _stopOnNextLine = true; // automatically stop for each new starting workflow

        public bool StopOnNextLine { get => _stopOnNextLine; set => _stopOnNextLine = value; }
        

        AutoResetEvent _waitUntilUserContinuesResetEvent = new AutoResetEvent(false);
        private IEnumerable<StackFrame> _currentStackFrames = new List<StackFrame>();
        private IEnumerable<Variable> _currentVariables = new List<Variable>();

        internal void Continue()
        {
            _waitUntilUserContinuesResetEvent.Set();
        }

        internal void NextLine()
        {
            this.StopOnNextLine = true;
            _waitUntilUserContinuesResetEvent.Set();
        }

        internal void StepIn()
        {
            _waitUntilUserContinuesResetEvent.Set();
        }

        internal void StepOut()
        {
            throw new NotImplementedException();
        }

        internal void WaitUntilUserContinues()
        {
            _waitUntilWorkflowStopsResetEvent.Set();
            _waitUntilUserContinuesResetEvent.WaitOne();
        }

        internal bool OnActivityExecuted(string activityID, Dictionary<string, object> variableValues)
        {
            if(this.WorkflowDebugInformation.IsActivityConsidered(activityID))
            {
                _currentStackFrames = this.WorkflowDebugInformation.GetCurrentStackFrames(activityID);
                _currentVariables = variableValues.Select(x => new Variable(x.Key, x.Value != null ? x.Value.ToString() : null,0, x.Value != null ? x.Value.GetType().FullName : "- null -"));
                return this.StopOnNextLine || this.WorkflowDebugInformation.ActivityHasBreakpoint(activityID);
            }

            return false;
        }

        internal IEnumerable<StackFrame> GetCurrentStackFrames()
        {
            return _currentStackFrames;
        }

        internal List<Scope> GetCurrentScopes(int frameId)
        {
            var scopes = new List<Scope>();
            scopes.Add(new Scope("Local", frameId,false));
            return scopes;
        }

        internal IEnumerable<Variable> GetCurrentVariables(int variablesReference)
        {
            return _currentVariables;
        }

       
    }
}
