using Microsoft.Msagl.Core.Geometry.Curves;
using Microsoft.Msagl.Drawing;
using System;
using System.Activities;
using System.Activities.Core.Presentation;
using System.Activities.Presentation;
using System.Activities.Presentation.Model;
using System.Activities.Presentation.Services;
using System.Activities.Presentation.View;
using System.Activities.Statements;
using System.Collections;
using System.Linq;
using System.Runtime.Versioning;
using System.Windows;
using System.Windows.Threading;

namespace TxFlow.WFBuilder.Layout
{
    internal class FlowChartLayouter
    {
        private ViewStateService service;
        private ModelService modelService;
        private WorkflowDesigner wd;

        public string WorkflowXaml { get; private set; }

        private MyEditorService expressionEditorService;

        private void assignIdRecursive(Activity activity)
        {
            foreach (Activity childActivity in WorkflowInspectionServices.GetActivities(activity))
            {
                assignIdRecursive(childActivity);
            }
        }

        public FlowChartLayouter(ActivityBuilder builder)
        {
            // register metadata
            (new DesignerMetadata()).Register();
            wd = new WorkflowDesigner();
            //https://blogs.msdn.microsoft.com/workflowteam/2016/07/20/building-c-expressions-support-and-intellisense-in-the-rehosted-workflow-designer/
            DesignerConfigurationService configurationService = wd.Context.Services.GetService<DesignerConfigurationService>();
            configurationService.TargetFrameworkName = new FrameworkName(".NETFramework", new System.Version(4, 5));
            configurationService.LoadingFromUntrustedSourceEnabled = true;

            // Create ExpressionEditorService 
            this.expressionEditorService = new MyEditorService();

            // Publish the instance of MyEditorService.
            wd.Context.Services.Publish<IExpressionEditorService>(this.expressionEditorService);

            ExpressionTextBox.RegisterExpressionActivityEditor("C#", typeof(CustomCSharpExpressionEditor), createExpressionFromStringCallback);

            wd.Load(builder);


            

            //System.Activities.Presentation.Expressions.


            new Window()
            {
                Content = wd.View,
                Visibility = Visibility.Hidden
            }.Show();

            wd.View.Dispatcher.BeginInvoke(DispatcherPriority.ApplicationIdle, new Action(this.arrangeLayout));
        }

        private ActivityWithResult createExpressionFromStringCallback(string expressionText, bool useLocationExpression, Type expressionType)
        {
            return null;
        }

        private void arrangeLayout()
        {
            service = wd.Context.Services.GetService<ViewStateService>();
            modelService = wd.Context.Services.GetService<ModelService>();

            Microsoft.Msagl.Drawing.Graph graph = new Microsoft.Msagl.Drawing.Graph("graph");
            graph.InitNodeMap(new Hashtable());

            var fcModelItem = modelService.Find(modelService.Root, typeof(Flowchart)).FirstOrDefault();
            var fc = fcModelItem.GetCurrentValue() as Flowchart;


            var rootModelItem = this.getModelItem(fc);
            System.Windows.Size shapeSize = (System.Windows.Size)service.RetrieveViewState(fcModelItem, "ShapeSize"); // ConnectorLocation
            Microsoft.Msagl.Drawing.Node b = new Microsoft.Msagl.Drawing.Node(Guid.NewGuid().ToString());
            b.NodeBoundaryDelegate = new GetNodeBoundary(shapeSize).Delegate;
            b.UserData = rootModelItem;
            graph.AddNode(b);


            createFlowChartRecursive(graph, b, fc.StartNode);

            Microsoft.Msagl.GraphViewerGdi.GraphRenderer redner = new Microsoft.Msagl.GraphViewerGdi.GraphRenderer(graph);
            redner.CalculateLayout();

            foreach (var node in graph.Nodes)
            {
                var modelItem = (node.UserData as ModelItem);

                using (ModelEditingScope scope = modelItem.BeginEdit())
                {
                    var tmp = service.RetrieveAllViewState(modelItem);
                    service.RemoveViewState(modelItem, "ShapeLocation");
                    //var location = new System.Windows.Point((graph.Width / 2) + node.Pos.X - (node.Width / 2), graph.Top - node.Pos.Y);
                    var location = new System.Windows.Point(-graph.BoundingBox.Left + node.Pos.X - (node.Width / 2), graph.BoundingBox.Top - node.Pos.Y - (node.Height / 2));
                    service.StoreViewState(modelItem, "ShapeLocation", location);

                    scope.Complete();

                }
            }



            wd.Flush();

            this.WorkflowXaml = wd.Text;
            Dispatcher.CurrentDispatcher.InvokeShutdown();
        }

        private void createFlowChartRecursive(Graph graph, Node predecessorNode, FlowNode currentNode)
        {
            if (currentNode == null)
            {
                return;
            }

            var modelItem = this.getModelItem(currentNode);


            Size shapeSize = (System.Windows.Size)service.RetrieveViewState(modelItem, "ShapeSize"); // ConnectorLocation
            Node b = new Node(Guid.NewGuid().ToString());
            b.NodeBoundaryDelegate = new GetNodeBoundary(shapeSize).Delegate;
            b.UserData = modelItem;
            graph.AddNode(b);

            var edge = graph.AddEdge(predecessorNode.Id, b.Id);

            if (currentNode is FlowStep)
            {
                createFlowChartRecursive(graph, b, (currentNode as FlowStep).Next);
            }
            else if (currentNode is FlowDecision)
            {
                createFlowChartRecursive(graph, b, (currentNode as FlowDecision).True);
                createFlowChartRecursive(graph, b, (currentNode as FlowDecision).False);
            }
            else if (typeof(FlowSwitch<>).IsAssignableFrom(currentNode.GetType().GetGenericTypeDefinition()))
            {
                var prop = currentNode.GetType().GetProperty("Cases");
                var nullableKeyDictionary = prop.GetValue(currentNode, null);
                var nodes = (IEnumerable)nullableKeyDictionary.GetType().GetProperty("Values").GetValue(nullableKeyDictionary, null);

                var defaultCase = currentNode.GetType().GetProperty("Default").GetValue(currentNode, null);
                createFlowChartRecursive(graph, b, (FlowNode)defaultCase);


                foreach (var node in nodes)
                {
                    createFlowChartRecursive(graph, b, (FlowNode)node);
                }
            }
        }

        private ModelItem getModelItem<T>(T activity)
        {
            var all = this.modelService.Find(this.modelService.Root, typeof(T));

            return all.Single(x => object.ReferenceEquals(x.GetCurrentValue(), activity));
        }

        private class GetNodeBoundary
        {
            private readonly ICurve _curve;

            public GetNodeBoundary(System.Windows.Size size)
            {
                _curve = CurveFactory.CreateRectangle(size.Width, size.Height, new Microsoft.Msagl.Core.Geometry.Point());
            }

            private ICurve getNodeBoundary(Microsoft.Msagl.Drawing.Node node)
            {
                return this._curve;
            }

            public DelegateToSetNodeBoundary Delegate
            {
                get
                {
                    return new DelegateToSetNodeBoundary(this.getNodeBoundary);
                }
            }
        }
    }
}
