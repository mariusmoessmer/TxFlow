using System;
using System.Activities;
using System.Activities.Statements;
using System.Collections.Generic;
using System.Reflection;
using TxFlow.Debug.ValueObjects;

namespace TxFlow.WFBuilder
{
    public class FlowChartBuilder
    {
        private readonly Flowchart _flowChart = null;
        private readonly FlowChartBuilder _parent = null;
        private readonly WorkflowPath _workflowPath = null;

        internal FlowChartBuilder(FlowChartBuilder parent, string displayName = "FlowChart")
        {
            _parent = parent;
            _flowChart = new Flowchart()
            {
                DisplayName = displayName,
            };

            _workflowPath = new WorkflowPath(this);
        }

        public FlowChartBuilder Parent =>  _parent;

        public WorkflowPath WorkflowPath => _workflowPath;


        public virtual List<Tuple<Activity, ActivitySourceLocationVO>> DebugLocations
        {
            get
            {
                return this.Parent.DebugLocations;
            }
        }


        internal Flowchart FlowChart
        {
            get
            {
                return this._flowChart;
            }
        }

        public string FlowChartPathText
        {
            get
            {
                var currentFlowChartBuilder = this;

                string result = string.Empty;

                while(currentFlowChartBuilder != null)
                {
                    result = currentFlowChartBuilder._flowChart.DisplayName + "->" + result;
                    currentFlowChartBuilder = currentFlowChartBuilder._parent;
                }

                return result.Substring(0, result.Length-2);
            }
        }

        private HashSet<string> _declaredVariables = new HashSet<string>();

        protected FlowChartBuilder getFlowChartWhichDeclaresVariable(string variableName)
        {
            if (_declaredVariables.Contains(variableName))
            {
                return this;
            }

            if (_parent != null)
            {
                return _parent.getFlowChartWhichDeclaresVariable(variableName);
            }

            return null;
        }

        internal void DeclareVariable(string fullTypeName, string variableName)
        {
            FlowChartBuilder flowChartWithDeclaresVariable = this.getFlowChartWhichDeclaresVariable(variableName);
            if(flowChartWithDeclaresVariable != null)
            {
                throw new Exception("Error declaring variable '" + variableName + "' in FlowChart "+this.FlowChartPathText+" because it is already declared in " + flowChartWithDeclaresVariable.FlowChartPathText);
            }

            Type type = WFActivities.WFActivityTypeHelper.ResolveType(fullTypeName);
            this._flowChart.Variables.Add(Variable.Create(variableName, type, VariableModifiers.Mapped));
            this._declaredVariables.Add(variableName);
        }
    }
}
