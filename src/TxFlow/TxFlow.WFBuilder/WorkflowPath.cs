using Microsoft.CSharp.Activities;
using System;
using System.Activities;
using System.Activities.Expressions;
using System.Activities.Statements;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using TxFlow.Debug.ValueObjects;
using TxFlow.WFActivities;

namespace TxFlow.WFBuilder
{
    public class WorkflowPath
    {
        private readonly FlowChartBuilder _flowChartBuilder;
        private Action<FlowNode> _assignNextFlowNodeAction;

        public string FlowChartDisplayName
        {
            get
            {
                return this._flowChartBuilder.FlowChart.DisplayName;
            }
        }

        public WorkflowPath(FlowChartBuilder flowChartBuilder, Action<FlowNode> assignNextFlowNodeAction = null)
        {
            this._flowChartBuilder = flowChartBuilder;

            Action<FlowNode> tmp = (node) => { this._flowChartBuilder.FlowChart.StartNode = node; };
            this._assignNextFlowNodeAction = assignNextFlowNodeAction ?? tmp;

        }
        

        private void registerNode(FlowNode node, Tuple<Activity, ActivitySourceLocationVO> debugLocationForActivity = null)
        {
            this._flowChartBuilder.FlowChart.Nodes.Add(node); // essential for designer
            if (debugLocationForActivity != null)
            {
                this._flowChartBuilder.DebugLocations.Add(debugLocationForActivity);
            }
        }

        private void nextAction(Activity activity, ActivitySourceLocationVO debugLocation = null)
        {
            var flowStep = new FlowStep()
            {
                Action = activity
            };

            Tuple<Activity, ActivitySourceLocationVO> debugLocationForActivity = null;
            if (debugLocation != null)
            {
                debugLocationForActivity = Tuple.Create(activity, debugLocation);
            }
            
            registerNode(flowStep, debugLocationForActivity);

            assignNextNode(flowStep, (node) => flowStep.Next = node);
        }

        protected void assignNextNode(FlowNode node, Action<FlowNode> assignNextFlowNodeAction)
        {
            this._assignNextFlowNodeAction(node);
            this._assignNextFlowNodeAction = assignNextFlowNodeAction;
        }

        public void ActivityInvocation(ActivitySourceLocationVO debugLocation, string activityName, IEnumerable<string> genericTypeNamnes, IEnumerable<object> parameterValues, Tuple<string, string> result = null)
        {
            OutArgument outArg = result != null ? createOutArgument(result.Item1, result.Item2) : null ;

            //var activity = ActivityFactory.Instance.CreateActivity(new CreateActivityParams()
            var activity = WFActivityTypeHelper.CreateActivity(new CreateActivityParams()
            {
                ActivityName = activityName,
                ParameterValues = parameterValues,
                OutArg = outArg,
                GenericTypes = genericTypeNamnes.Select(x => WFActivityTypeHelper.ResolveType(x)).ToArray()
            });

            nextAction(activity, debugLocation);
        }

        public void Assign(ActivitySourceLocationVO debugLocation, string leftExpression, string leftExpressionType, string rightExpression, string rightExpressionType)
        {
            var activityWithResult = WFActivityTypeHelper.CreateArgumentExpression(rightExpression, rightExpressionType);
            this.Assign(debugLocation, leftExpression, leftExpressionType, activityWithResult);
        }

        public void Assign(ActivitySourceLocationVO debugLocation, string leftExpression, string leftExpressionType, ActivityWithResult activityWithResult)
        {

            OutArgument outArgumentObject = createOutArgument(leftExpression, leftExpressionType);


            //var outArgumentObject = (OutArgument)Activator.CreateInstance(outArgumentType, WorkflowSyntaxFactory.ArgumentExpression(leftExpression, leftExpressionType));


            nextAction(new Assign()
            {
                To = outArgumentObject,
                Value = (InArgument)createWithGenericType(typeof(InArgument<>), activityWithResult.ResultType, activityWithResult),
            }, debugLocation);
        }

        private OutArgument createOutArgument(string leftExpression, string leftExpressionType)
        {
            Type leftType = WFActivities.WFActivityTypeHelper.ResolveType(leftExpressionType);
            var outArgumentType = typeof(OutArgument<>).MakeGenericType(leftType);


            FlowChartBuilder currentFlowChartBuilder = this._flowChartBuilder;
            while (currentFlowChartBuilder != null)
            {
                Variable v = currentFlowChartBuilder.FlowChart.Variables.FirstOrDefault(x => x.Name == leftExpression);
                if (v != null)
                {
                    return (OutArgument)Activator.CreateInstance(outArgumentType, v);
                }

                currentFlowChartBuilder = currentFlowChartBuilder.Parent;
            }

            // we have reached root-flowchart
            // try to get outargument out of arguments of workflow
            return (OutArgument)Activator.CreateInstance(outArgumentType, createWithGenericType(typeof(ArgumentReference<>), leftType, leftExpression));
        }

        protected static object createWithGenericType(Type mainType, Type genericType, params object[] constructorParams)
        {
            var type = mainType.MakeGenericType(genericType);
            return Activator.CreateInstance(type, constructorParams);
        }

        public void While(string conditionExpression, Action<WorkflowPath> action)
        {
            var whileAct = new System.Activities.Statements.While(new CSharpValue<bool>(conditionExpression))
            {
                Body = this.FlowChart(null, "WhileBody", action, false)
            };

            this.nextAction(whileAct);
        }

        public void DoWhile(string conditionExpression, Action<WorkflowPath> action)
        {
            var doWhileAct = new System.Activities.Statements.DoWhile(new CSharpValue<bool>(conditionExpression))
            {
                Body = this.FlowChart(null, "DoWhileBody", action, false)
            };

            this.nextAction(doWhileAct);
        }

        public void Throw(ActivityWithResult expression)
        {
            Throw th = new System.Activities.Statements.Throw();
            th.Exception = new InArgument<Exception>((Activity<Exception>)expression);

            this.nextAction(th);
        }


        public void TryCatchFinally(Action<WorkflowPath> tryActionPath, IEnumerable<CatchBuilder> catchPaths, Action<WorkflowPath> finallyPath = null)
        {
            TryCatch activity = new TryCatch();
            activity.Try = this.FlowChart(null, "Try", tryActionPath, false);

            if(catchPaths != null)
            {
                foreach(var cPath in catchPaths)
                {
                    Type exceptionType = TypeInfo.GetType(cPath.TypeFullName, true);

                    var catchtype = typeof(Catch<>).MakeGenericType(exceptionType);
                    var actionType = typeof(ActivityAction<>).MakeGenericType(exceptionType);
                    var delegateInArgumentType = typeof(DelegateInArgument<>).MakeGenericType(exceptionType);


                    dynamic c = Activator.CreateInstance(catchtype);
                    dynamic action = Activator.CreateInstance(actionType);
                    dynamic delegateInArgument = Activator.CreateInstance(delegateInArgumentType);
                    delegateInArgument.Name = cPath.ExceptionVariableName;
                    action.Argument = delegateInArgument;
                    action.Handler = this.FlowChart(null, "Catch", cPath.WorkflowPathAction, false);
                    c.Action = action;

                    activity.Catches.Add(c);
                }
            }

            if(finallyPath != null)
            {
                activity.Finally = this.FlowChart(null, "Finally", finallyPath, false);
            }

            this.nextAction(activity);
        }


        public void FlowDecision(ActivitySourceLocationVO debugLocation, string conditionExpression, Action<WorkflowPath> trueAction, Action<WorkflowPath> falseAction)
        {

            var decisionActivity = new CSharpValue<bool>(conditionExpression);
            var flowDecision = new FlowDecision(decisionActivity);
            registerNode(flowDecision, Tuple.Create<Activity, ActivitySourceLocationVO>(decisionActivity, debugLocation));

            List<WorkflowPath> paths = new List<WorkflowPath>();
            assignNextNode(flowDecision, (node) =>
            {
                foreach(var i in paths)
                {
                    i.assignNextNode(node, (n) => throw new Exception("IF already merged"));
                }
            });

            //this._currentStep = END;

            WorkflowPath truePath = new WorkflowPath(this._flowChartBuilder, (node) => flowDecision.True = node);
            paths.Add(truePath);
            if (trueAction != null)
            {
                trueAction(truePath);
            }


            WorkflowPath falsePath = new WorkflowPath(this._flowChartBuilder, (node) => flowDecision.False = node);
            paths.Add(falsePath);
            if (falseAction != null)
            {
                falseAction(falsePath);
            }            
        }

        public void DeclareVariable(string fullTypeName, string variableName)
        {
            this._flowChartBuilder.DeclareVariable(fullTypeName, variableName);
        }

        public void Return()
        {
            this._assignNextFlowNodeAction = (n) =>
            {
                // NOP
                // TODO think about return in nested activities
            };
        }

        public void Switch(string fullTypeName, string expression, Action<WorkflowPath> defaultAction, List<Tuple<Action<WorkflowPath>, List<object>>> cases)
        {
            Type type = TypeInfo.GetType(fullTypeName);
            object flowSwitch = createWithGenericType(typeof(FlowSwitch<>), type);

            typeof(FlowSwitch<>).MakeGenericType(type).GetProperty("Expression", BindingFlags.Instance | BindingFlags.Public)
                .SetValue(flowSwitch, WFActivityTypeHelper.CreateArgumentExpression(expression, fullTypeName));

            List<WorkflowPath> paths = new List<WorkflowPath>();

            registerNode((FlowNode)flowSwitch);

            this.assignNextNode((FlowNode)flowSwitch, (node) =>
            {
                paths.ForEach(x => x.assignNextNode(node, (n) => throw new Exception("Switched already merged!")));
            });

            if (defaultAction != null)
            {
                WorkflowPath path = new WorkflowPath(this._flowChartBuilder, (node) =>
                            typeof(FlowSwitch<>).MakeGenericType(type).GetProperty("Default", BindingFlags.Instance | BindingFlags.Public)
                                .SetValue(flowSwitch, node));
                paths.Add(path);

                defaultAction(path);
            }
            // IDictionary < T, FlowNode >

            

            object dict = typeof(FlowSwitch<>).MakeGenericType(type).GetProperty("Cases", BindingFlags.Instance | BindingFlags.Public)
                .GetValue(flowSwitch);

            var method = dict.GetType().GetMethod("Add", new[] { type, typeof(FlowNode) });

            foreach (Tuple<Action<WorkflowPath>, List<object>> c in cases)
            {
                WorkflowPath path = new WorkflowPath(this._flowChartBuilder, (node) =>
                {
                    foreach (object key in c.Item2)
                    {
                        method.Invoke(dict, new[] { key, node });
                    }
                });

                paths.Add(path);

                c.Item1(path);
            }
        }

        public void Rethrow()
        {
            this.nextAction(new Rethrow());
        }

        public void Break()
        {
            // NOP
        }

        public Flowchart FlowChart(ActivitySourceLocationVO debugLocation, string activityName, Action<WorkflowPath> flowchartPath, bool addAsNextAction = true)
        {

            FlowChartBuilder builder = new FlowChartBuilder(this._flowChartBuilder, activityName);
            if(addAsNextAction)
            {
                this.nextAction(builder.FlowChart, debugLocation);
            }

            flowchartPath(builder.WorkflowPath);
            return builder.FlowChart;
        }
    }
}
