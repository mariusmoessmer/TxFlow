/*---------------------------------------------------------------------------------------------
 *  Copyright (c) Microsoft Corporation. All rights reserved.
 *  Licensed under the MIT License. See License.txt in the project root for license information.
 *--------------------------------------------------------------------------------------------*/
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using TxFlow.DebugAdapter.DebugInformation;
using TxFlow.DebugAdapter.Protocol;
using TxFlow.DebugAdapter.Service;

namespace TxFlow.DebugAdapter
{
	public class TxFlowDebugSession : DebugSession
	{
		private readonly string[] TXFLOW_EXTENSIONS = new String[] {
			".cs"
		};
		private const int MAX_CHILDREN = 100;

        private readonly WorkflowDebugService _workflowDebugService = null;

        public TxFlowDebugSession() : base()
        {
            _workflowDebugService = new WorkflowDebugService();
        }

        private void workflowInstanceStopped(object sender, Other.GenericEventArgs<WorkflowInstanceProxy> e)
        {
            SendEvent(CreateStoppedEvent("step", e.Data));
        }

        private StoppedEvent CreateStoppedEvent(string reason, WorkflowInstanceProxy ti, string text = null)
        {
            return new StoppedEvent((int)ti.ThreadId, reason, text);
        }

        public override void Initialize(Response response, dynamic args)
		{
			SendResponse(response, new Capabilities() {
				// This debug adapter does not need the configurationDoneRequest.
				supportsConfigurationDoneRequest = false,

				// This debug adapter does not support function breakpoints.
				supportsFunctionBreakpoints = false,

				// This debug adapter doesn't support conditional breakpoints.
				supportsConditionalBreakpoints = false,

				// This debug adapter does not support a side effect free evaluate request for data hovers.
				supportsEvaluateForHovers = false,

				// This debug adapter does not support exception breakpoint filters
				exceptionBreakpointFilters = new dynamic[0]
			});

			// Mono Debug is ready to accept breakpoints immediately
			SendEvent(new InitializedEvent());
		}

		

		public override void Attach(Response response, dynamic args)
		{
			// validate argument 'address'
			string host = getString(args, "address");

            Uri hostUri = null;
            if (host == null || !Uri.TryCreate(host, UriKind.RelativeOrAbsolute, out hostUri)) {
				SendErrorResponse(response, 3007, $"Property 'address' with value '{host}' is missing, empty or invalid.");
				return;
			}

            var projectDir = getString(args, "projectDir");
            if (projectDir == null || !Directory.Exists(projectDir))
            {
                SendErrorResponse(response, 3007, "Property 'projectDir' is missing or empty.");
                return;
            }
            DebugInformationProvider.Instance.InitializeProjectDir(projectDir);
            _workflowDebugService.HostService(hostUri);
            _workflowDebugService.WorkflowInstanceStopped += workflowInstanceStopped;

            SendResponse(response);
		}

        public override async void Launch(Response response, dynamic args)
        {
            await System.Threading.Tasks.Task.FromException(new InvalidOperationException("Launching is not possible"));
        }


        public override void Disconnect(Response response, dynamic args)
		{
            _workflowDebugService.Disconnect();
			SendResponse(response);
		}

		public override void Continue(Response response, dynamic args)
		{
            int workflowInstanceId = getInt(args, "threadId", 0);
            var proxy = _workflowDebugService.GetWorkflowInstanceProxy(workflowInstanceId);
            proxy.WaitForSuspend();
			SendResponse(response);
            proxy.Continue();            
		}

		public override void Next(Response response, dynamic args)
		{
            int workflowInstanceId = getInt(args, "threadId", 0);
            var proxy = _workflowDebugService.GetWorkflowInstanceProxy(workflowInstanceId);
            proxy.WaitForSuspend();
			SendResponse(response);
            proxy.NextLine();
		}

		public override void StepIn(Response response, dynamic args)
		{
            int workflowInstanceId = getInt(args, "threadId", 0);
            var proxy = _workflowDebugService.GetWorkflowInstanceProxy(workflowInstanceId);
            proxy.WaitForSuspend();
            SendResponse(response);
            proxy.StepIn();
		}

		public override void StepOut(Response response, dynamic args)
		{
            int workflowInstanceId = getInt(args, "threadId", 0);
            var proxy = _workflowDebugService.GetWorkflowInstanceProxy(workflowInstanceId);
            proxy.WaitForSuspend();
            proxy.StepOut();
        }

		public override void Pause(Response response, dynamic args)
		{
            int workflowInstanceId = getInt(args, "threadId", 0);

            this._workflowDebugService.Pause();

            SendResponse(response);
        }

		public override void SetExceptionBreakpoints(Response response, dynamic args)
		{
			SendResponse(response);
		}

		public override void SetBreakpoints(Response response, dynamic args)
		{
			string path = null;
			if (args.source != null) {
				string p = (string)args.source.path;
				if (p != null && p.Trim().Length > 0) {
					path = p;
				}
			}
			if (path == null) {
				SendErrorResponse(response, 3010, "setBreakpoints: property 'source' is empty or misformed", null, false, true);
				return;
			}

			if (!TXFLOW_EXTENSIONS.All(x => path.EndsWith(x))) {
				// we only support breakpoints in files txflow can handle
				SendResponse(response, new SetBreakpointsResponseBody());
				return;
			}

			int[] clientLines = args.lines.ToObject<int[]>();

            var breakpoints = DebugInformationProvider.Instance.SetBreakPoints(path, clientLines);
            
			SendResponse(response, new SetBreakpointsResponseBody(breakpoints));
		}


        private WorkflowInstanceProxy _lastAccessedWorkflowInstance = null;
        private WorkflowInstanceProxy lastAccessedWorkflowInstance
        {
            get
            {
                var proxy = _lastAccessedWorkflowInstance;
                if (proxy == null)
                {
                    throw new Exception("Unexpected VSCode-behaviour: it should first call StackTrace and then scopes and then variables");
                }

                return proxy;
            }
        }

		public override void StackTrace(Response response, dynamic args)
		{
            int workflowInstanceId = getInt(args, "threadId", 0);
            var proxy = _workflowDebugService.GetWorkflowInstanceProxy(workflowInstanceId);
            _lastAccessedWorkflowInstance = proxy;
            proxy.WaitForSuspend();

            var stackFrames = proxy.GetCurrentStackFrames();

            SendResponse(response, new StackTraceResponseBody(stackFrames));
		}

		public override void Source(Response response, dynamic arguments) {
			SendErrorResponse(response, 1020, "No source available");
		}

		public override void Scopes(Response response, dynamic args) {

			int frameId = getInt(args, "frameId", 0); // frameId == worklowInstanceId


            var proxy = lastAccessedWorkflowInstance;
            proxy.WaitForSuspend();

            List<Scope> scopes = proxy.GetCurrentScopes(frameId);

            SendResponse(response, new ScopesResponseBody(scopes));
		}

		public override void Variables(Response response, dynamic args)
		{
			int variablesReference = getInt(args, "variablesReference", -1);
			if (variablesReference == -1) {
				SendErrorResponse(response, 3009, "variables: property 'variablesReference' is missing", null, false, true);
				return;
			}

            var proxy = lastAccessedWorkflowInstance;
            proxy.WaitForSuspend();

            IEnumerable<Variable> variables = proxy.GetCurrentVariables(variablesReference);

			SendResponse(response, new VariablesResponseBody(variables));
		}
		public override void Threads(Response response, dynamic args)
		{
			SendResponse(response, new ThreadsResponseBody(_workflowDebugService.GetAllRunningWorkflowsAsThreads()));
		}

		

		//---- private ------------------------------------------

		//private Variable CreateVariable(ObjectValue v)
		//{
		//	var dv = v.DisplayValue;
		//	if (dv.Length > 1 && dv [0] == '{' && dv [dv.Length - 1] == '}') {
		//		dv = dv.Substring (1, dv.Length - 2);
		//	}
		//	return new Variable(v.Name, dv, v.TypeName, v.HasChildren ? _variableHandles.Create(v.GetAllChildren()) : 0);
		//}

		private static bool getBool(dynamic container, string propertyName, bool dflt = false)
		{
			try {
				return (bool)container[propertyName];
			}
			catch (Exception) {
				// ignore and return default value
			}
			return dflt;
		}

		private static int getInt(dynamic container, string propertyName, int dflt = 0)
		{
			try {
				return (int)container[propertyName];
			}
			catch (Exception) {
				// ignore and return default value
			}
			return dflt;
		}

		private static string getString(dynamic args, string property, string dflt = null)
		{
			var s = (string)args[property];
			if (s == null) {
				return dflt;
			}
			s = s.Trim();
			if (s.Length == 0) {
				return dflt;
			}
			return s;
		}

		//-----------------------

        #region unneeded

        public override void Evaluate(Response response, dynamic args)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
