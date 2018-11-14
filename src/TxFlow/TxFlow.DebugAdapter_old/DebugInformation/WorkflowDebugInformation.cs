using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TxFlow.Debug.ValueObjects;
using TxFlow.DebugAdapter.Protocol;

namespace TxFlow.DebugAdapter.DebugInformation
{
    class WorkflowDebugInformation
    {
        private Dictionary<string, CSharpDebugLocationVO> _activityLineAndColumnMapping = new Dictionary<string, CSharpDebugLocationVO>();
        private string _workflowName = null;

        internal IEnumerable<StackFrame> GetCurrentStackFrames(string activityID)
        {
            var cSharpDebugLocation = this._activityLineAndColumnMapping[activityID];

            return new[] { new StackFrame(123, cSharpDebugLocation.FlowChartDisplayName, new Source(this._workflowName, cSharpDebugLocation.SourceFilePath, 0, "normal"), cSharpDebugLocation.Row, cSharpDebugLocation.Column, "normal") };
        }

        private HashSet<int> _breakpointLines = new HashSet<int>();

        public WorkflowDebugInformation(CSharpWorkflowDebugVO des)
        {
            this._workflowName = des.WorkflowName;
            this._activityLineAndColumnMapping = des.ActivitySourceLocation.ToDictionary(x => x.ActivityId, x => x.Location);
        }

        public string WorkflowName { get => _workflowName; set => _workflowName = value; }

        internal int[] SetBreakPoints(string path, int[] clientLines)
        {
            var allPossibleLines = _activityLineAndColumnMapping.Where(x=> Path.GetFullPath(x.Value.SourceFilePath).ToLower() == Path.GetFullPath(path).ToLower()).Select(x => x.Value.Row).Distinct().ToArray();

            if (allPossibleLines.Length < 1)
            {
                _breakpointLines = new HashSet<int>();
            }
            else
            {
                _breakpointLines = new HashSet<int>(clientLines.Select(x =>
                {
                    if (allPossibleLines.Contains(x))
                    {
                        return x;
                    }
                    else
                    {
                    // find the one with smallest deviation
                    KeyValuePair<int, int>? smallest = null;
                        foreach (var possibleLine in allPossibleLines)
                        {
                            int deviation = possibleLine - x;
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
            int line = _activityLineAndColumnMapping[activityID].Row;
            return _breakpointLines.Contains(line);
        }

        internal bool IsActivityConsidered(string activityID)
        {
            return _activityLineAndColumnMapping.ContainsKey(activityID);
        }
    }
}
