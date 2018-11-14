using Microsoft.VisualStudio.Shared.VSCodeDebugProtocol;
using Microsoft.VisualStudio.Shared.VSCodeDebugProtocol.Messages;
using System.IO;
using Microsoft.VisualStudio.Shared.VSCodeDebugProtocol.Utilities;
using System;
using System.Linq;
using TxFlow.Debug.Adapter.DebugInformation;
using TxFlow.Debug.Adapter.Service;
using System.Collections.Generic;

namespace TxFlow.Debug.Adapter
{
    internal class TxFlowDebugAdapter : DebugAdapterBase
    {
        private readonly WorkflowDebugService _workflowDebugService = new WorkflowDebugService();

        public TxFlowDebugAdapter(Stream stdIn, Stream stdOut)
        {
            base.InitializeProtocolClient(stdIn, stdOut);
        }


        protected override InitializeResponse HandleInitializeRequest(InitializeArguments arguments)
        {
            //if (arguments.LinesStartAt1 == true)
            //    this.clientsFirstLine = 1;

            this.Protocol.SendEvent(new InitializedEvent());

            return new InitializeResponse(
                supportsConfigurationDoneRequest: false,
                supportsSetVariable: false,
                supportsDebuggerProperties: false,
                supportsModulesRequest: false,
                supportsSetExpression: false,
                supportsExceptionOptions: false,
                supportsExceptionConditions: false,
                supportsExceptionInfoRequest: false,
                supportsValueFormattingOptions: false,
                supportsEvaluateForHovers: false
            );
        }

        protected override DisconnectResponse HandleDisconnectRequest(DisconnectArguments arguments)
        {
            _workflowDebugService.Disconnect();
            return new DisconnectResponse();
        }

        protected override AttachResponse HandleAttachRequest(AttachArguments arguments)
        {
            // validate argument 'address'
            string host = arguments.ConfigurationProperties.GetValueAsString("address");

            Uri hostUri = null;
            if (host == null || !Uri.TryCreate(host, UriKind.RelativeOrAbsolute, out hostUri))
            {
                throw new ProtocolException($"Property 'address' with value '{host}' is missing, empty or invalid.");
            }

            _workflowDebugService.HostService(hostUri);
            _workflowDebugService.WorkflowInstanceStopped += workflowInstanceStopped;
            return new AttachResponse();
        }

        private void workflowInstanceStopped(object sender, Other.GenericEventArgs<WorkflowInstanceProxy> e)
        {
            this.Protocol.SendEvent(new StoppedEvent(StoppedEvent.ReasonValue.Step, null, e.Data.ThreadId));
        }

        protected override ContinueResponse HandleContinueRequest(ContinueArguments arguments)
        {
            int workflowInstanceId = arguments.ThreadId;
            var proxy = _workflowDebugService.GetWorkflowInstanceProxy(workflowInstanceId);
            proxy.WaitForSuspend();
            //Protocol.SendEvent(new ContinuedEvent());
            proxy.Continue();
            return new ContinueResponse();
        }

        protected override NextResponse HandleNextRequest(NextArguments arguments)
        {
            int workflowInstanceId = arguments.ThreadId;
            var proxy = _workflowDebugService.GetWorkflowInstanceProxy(workflowInstanceId);
            proxy.WaitForSuspend();
            //Protocol.SendEvent(new Next());
            proxy.NextLine();
            return new NextResponse();
        }

   
        protected override StepInResponse HandleStepInRequest(StepInArguments arguments)
        {
            int workflowInstanceId = arguments.ThreadId;
            var proxy = _workflowDebugService.GetWorkflowInstanceProxy(workflowInstanceId);
            proxy.WaitForSuspend();
            //SendResponse(response);
            proxy.StepIn();
            return new StepInResponse();
        }

        protected override StepOutResponse HandleStepOutRequest(StepOutArguments arguments)
        {
            int workflowInstanceId = arguments.ThreadId;
            var proxy = _workflowDebugService.GetWorkflowInstanceProxy(workflowInstanceId);
            proxy.WaitForSuspend();
            proxy.StepOut();
            return new StepOutResponse();
        }

        protected override PauseResponse HandlePauseRequest(PauseArguments arguments)
        {
            int workflowInstanceId = arguments.ThreadId;

            this._workflowDebugService.Pause();

            return new PauseResponse();
        }

        protected override SetBreakpointsResponse HandleSetBreakpointsRequest(SetBreakpointsArguments args)
        {
            string path = args.Source.Path;
            if (path == null)
            {
                throw new ProtocolException("setBreakpoints: property 'source' is empty or misformed");
            }

            var breakpoints = DebugInformationProvider.Instance.SetBreakPoints(path, args.Breakpoints);

            return new SetBreakpointsResponse(breakpoints);
        }

        protected override SetExceptionBreakpointsResponse HandleSetExceptionBreakpointsRequest(SetExceptionBreakpointsArguments arguments)
        {
            return new SetExceptionBreakpointsResponse();
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

        protected override StackTraceResponse HandleStackTraceRequest(StackTraceArguments arguments)
        {
            int workflowInstanceId = arguments.ThreadId;
            var proxy = _workflowDebugService.GetWorkflowInstanceProxy(workflowInstanceId, false);
            if(proxy == null)
            {
                // Visual studio sometimes requests stacktrace for already completed wfs
                return new StackTraceResponse();
            }else
            {
                _lastAccessedWorkflowInstance = proxy;
                proxy.WaitForSuspend();

                var stackFrames = proxy.GetCurrentStackFrames();

                return new StackTraceResponse(stackFrames.ToList());
            }
        }

        protected override ScopesResponse HandleScopesRequest(ScopesArguments arguments)
        {
            int frameId = arguments.FrameId; // frameId == worklowInstanceId
            var proxy = lastAccessedWorkflowInstance;
            proxy.WaitForSuspend();

            List<Scope> scopes = proxy.GetCurrentScopes(frameId);
            return new ScopesResponse(scopes);
        }

        protected override VariablesResponse HandleVariablesRequest(VariablesArguments arguments)
        {
            int variablesReference = arguments.VariablesReference;

            var proxy = lastAccessedWorkflowInstance;
            proxy.WaitForSuspend();

            IEnumerable<Variable> variables = proxy.GetCurrentVariables(variablesReference);

            return new VariablesResponse(variables.ToList());            
        }

        protected override ThreadsResponse HandleThreadsRequest(ThreadsArguments arguments)
        {
            return new ThreadsResponse(_workflowDebugService.GetAllRunningWorkflowsAsThreads().ToList());
        }

        protected override EvaluateResponse HandleEvaluateRequest(EvaluateArguments arguments)
        {
            return new EvaluateResponse();
        }
    }
}