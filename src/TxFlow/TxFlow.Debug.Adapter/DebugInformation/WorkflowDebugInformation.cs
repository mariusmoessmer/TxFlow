using Microsoft.VisualStudio.Shared.VSCodeDebugProtocol.Messages;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TxFlow.Debug.ValueObjects;

namespace TxFlow.Debug.Adapter.DebugInformation
{
    class WorkflowDebugInformation
    {
        private readonly Dictionary<string, ActivitySourceLocationVO> _activityLineAndColumnMapping = new Dictionary<string, ActivitySourceLocationVO>();
        private readonly string _workflowName = null;
        private readonly string _csharpFilePath = null;

        internal IEnumerable<StackFrame> GetCurrentStackFrames(string activityID)
        {
            var cSharpDebugLocation = this._activityLineAndColumnMapping[activityID];

            return new[] { new StackFrame(123, cSharpDebugLocation.ParentFlowChartName, cSharpDebugLocation.Line, cSharpDebugLocation.Column, new Source(this._workflowName, this._csharpFilePath, 0, Source.PresentationHintValue.Normal), presentationHint: StackFrame.PresentationHintValue.Normal) };
        }

        private HashSet<int> _breakpointLines = new HashSet<int>();

        public WorkflowDebugInformation(string csharpFilePath, WorkflowDebugSourceMapVO des)
        {
            this._csharpFilePath = csharpFilePath;
            this._workflowName = des.WorkflowName;
            this._activityLineAndColumnMapping = des.ActivitySourceLocations.ToDictionary(x => x.ActivityId, x => x);
        }

        public string WorkflowName { get => _workflowName; }

        internal int[] SetBreakPoints(string path, List<SourceBreakpoint> sourceBreakpoints)
        {
            if (Path.GetFullPath(this._csharpFilePath).ToLower() != Path.GetFullPath(path).ToLower())
            {
                _breakpointLines = new HashSet<int>();
                return null;
            }
            else
            {
                var allPossibleLines = _activityLineAndColumnMapping.Select(x => x.Value.Line).Distinct().ToArray();
                _breakpointLines = new HashSet<int>(sourceBreakpoints.Select(x =>
                {
                    if (allPossibleLines.Contains(x.Line))
                    {
                        return x.Line;
                    }
                    else
                    {
                    // find the one with smallest deviation
                    KeyValuePair<int, int>? smallest = null;
                        foreach (var possibleLine in allPossibleLines)
                        {
                            int deviation = possibleLine - x.Line;
                            if (smallest == null
                                || Math.Abs(smallest.Value.Value) > Math.Abs(deviation)
                                || (Math.Abs(smallest.Value.Value) == Math.Abs(deviation) && deviation > 0))
                            {
                                smallest = new KeyValuePair<int, int>(possibleLine, deviation);
                            }
                        }

                        return smallest.Value.Key;
                    }
                }));
            }

            return _breakpointLines.ToArray();
        }

        internal bool ActivityHasBreakpoint(string activityID)
        {
            int line = _activityLineAndColumnMapping[activityID].Line;
            return _breakpointLines.Contains(line);
        }

        internal bool IsActivityConsidered(string activityID)
        {
            return _activityLineAndColumnMapping.ContainsKey(activityID);
        }
    }
}
