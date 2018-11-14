//----------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
//----------------------------------------------------------------
using System;
using System.Activities;
using System.Activities.Core.Presentation;
using System.Activities.Debugger;
using System.Activities.Presentation;
using System.Activities.Presentation.Debug;
using System.Activities.Presentation.Services;
using System.Activities.Tracking;
using System.Activities.XamlIntegration;
using System.Collections.Generic;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;
using System.Linq;
using System.ServiceModel;
using System.Activities.Presentation.View;
using TxFlow.WFBuilder.Layout;
using System.Runtime.Versioning;
using System.Xaml;
using System.IO;
using System.Activities.Validation;
using System.Reflection;
using TxFlow.Debug.WFTracking;

namespace Microsoft.Samples.VisualWorkflowTracking
{
    /// <summary>
    /// Interaction logic for WorkflowDesignerHost.xaml
    /// </summary>
    public partial class WorkflowDesignerHost : UserControl
    {
        public WorkflowDesigner WorkflowDesigner { get; set; }
        public IDesignerDebugView DebuggerService { get; set; }

        TextBox tx;
        Dictionary<int, SourceLocation> textLineToSourceLocationMap;
        int i = 0;

        public WorkflowDesignerHost()
        {
            InitializeComponent();
            RegisterMetadata();
            AddWorkflowDesigner();
            AddTrackingTextbox();
        }

        //private string workflowPath = "Workflow.xaml";
        private string workflowPath = "C:\\HolidayApproval.TxFlow\\bin\\HolidayApprovalWorkflow.xaml";
        private Assembly _ass2;

        private void RegisterMetadata()
        {
            (new DesignerMetadata()).Register();
        }

        private void AddWorkflowDesigner()
        {
            var wd = new WorkflowDesigner();


            DesignerConfigurationService configurationService = wd.Context.Services.GetService<DesignerConfigurationService>();
            configurationService.TargetFrameworkName = new FrameworkName(".NETFramework", new System.Version(4, 5));
            configurationService.LoadingFromUntrustedSourceEnabled = true;


            var expressionEditorService = new MyEditorService();

            // Publish the instance of MyEditorService.
            wd.Context.Services.Publish<IExpressionEditorService>(expressionEditorService);

            ExpressionTextBox.RegisterExpressionActivityEditor("C#", typeof(CustomCSharpExpressionEditor), (x, y, z) => null);

            
            //var ass2 = System.Reflection.Assembly.LoadFile(@"C:\Users\Marius\Source\Repos\TxFlow\TxFlow\HolidayApprovalWFConsoleApp\bin\Debug\HolidayApprovalWFConsoleApp.exe");
            var ass = System.Reflection.Assembly.LoadFrom(@"C:\HolidayApproval.TxFlow\bin\HolidayApproval.Activities.dll");
            _ass2 = System.Reflection.Assembly.LoadFrom(@"C:\HolidayApproval.TxFlow\bin\HolidayApproval.Entities.dll");

                using (var tmp = new XamlXmlReader(workflowPath, new XamlSchemaContext()))
                {
                    using (var br = System.Activities.XamlIntegration.ActivityXamlServices.CreateBuilderReader(tmp))
                    {
                        var ab = System.Xaml.XamlServices.Load(br) as System.Activities.ActivityBuilder;
                        wd.Load(ab.Implementation);
                    }
                }

            this.WorkflowDesigner = wd;
            this.DebuggerService = this.WorkflowDesigner.DebugManagerView;

            this.RehostGrid.Children.Add(this.WorkflowDesigner.View);

        }

        //Run the Workflow with the tracking participant
        public void RunWorkflow()
        {
            Guid workflowInstanceID = Guid.NewGuid();

            var root = GetRuntimeExecutionRoot();
               WorkflowInvoker instance = new WorkflowInvoker(root);

               //Mapping between the Object and Line No.
                Dictionary<object, SourceLocation> wfElementToSourceLocationMap = UpdateSourceLocationMappingInDebuggerService();

                //Mapping between the Object and the Instance Id
                Dictionary<string, Activity> activityIdToWfElementMap = BuildActivityIdToWfElementMap(wfElementToSourceLocationMap);


                # region Set up Custom Tracking
                VisualTrackingParticipant simTracker = new VisualTrackingParticipant(root);

                # endregion

                //As the tracking events are received
                simTracker.TrackingRecordReceived += (trackingParticpant, trackingEventArgs) =>
                    {


                        //_workflowDebugService.ActivityExecuted(workflowInstanceID, trackingEventArgs.ActivityId, trackingEventArgs.Variables.ToDictionary(x => x.Key, y => y.Value));

                        Activity activity;
                        if (activityIdToWfElementMap.TryGetValue(trackingEventArgs.ActivityId, out activity))
                        {
                            System.Diagnostics.Debug.WriteLine(
                                String.Format("<+=+=+=+> Activity Tracking Record Received for ActivityId: {0}, record: {1} ",
                                activity.Id,
                                trackingEventArgs.Record
                                )
                            );

                            ShowDebug(wfElementToSourceLocationMap[activity]);
                            

                            this.Dispatcher.Invoke(DispatcherPriority.SystemIdle, (Action)(() =>
                            {
                                //Textbox Updates
                                tx.AppendText(activity.DisplayName + "\n");
                                if(trackingEventArgs.Variables != null)
                                {
                                    tx.AppendText("Variables: " +Environment.NewLine
                                        + string.Join(Environment.NewLine + "\t", trackingEventArgs.Variables.Select(x => x.Key + ": " + x.Value ?? "[NULL]")) + Environment.NewLine);
                                }
                                tx.AppendText("******************\n");
                                textLineToSourceLocationMap.Add(i, wfElementToSourceLocationMap[activity]);
                                i = i + 2;
                                
                                //Add a sleep so that the debug adornments are visible to the user
                                System.Threading.Thread.Sleep(300);
                            }));

                        }
                    };

            //instance.Extensions.Add(simTracker);



            //instance.Extensions.Add(new TxFlow.Debug.WFTracking.DebugTrackingParticipant(root));
            Activity deserializedXamlWorkflow = root;

WorkflowInvoker workflowInvoker = new WorkflowInvoker(deserializedXamlWorkflow);
var debugExtension = new DebugTrackingParticipant(
        deserializedXamlWorkflow, "http://localhost:8071"
);
workflowInvoker.Extensions.Add(debugExtension);
workflowInvoker.Invoke();


            ThreadPool.QueueUserWorkItem(new WaitCallback((context) =>
                {
                    var type = _ass2.GetType("HolidayApproval.Entities.HolidayRequestEntity");
                    object hr = type.GetField("ForTesting").GetValue(null);


                    //Invoking the Workflow Instance with Input Arguments
                    instance.Invoke(new Dictionary<string, object> { { "holidayRequestEntity", hr}}, new TimeSpan(1,0,0));

                    //This is to remove the final debug adornment
                    this.Dispatcher.Invoke(DispatcherPriority.Render
                        , (Action)(() =>
                    {
                        this.WorkflowDesigner.DebugManagerView.CurrentLocation = new SourceLocation(workflowPath, 1,1,1,10);
                    }));

                }));
            
        }

        void ShowDebug(SourceLocation srcLoc)
        {
            this.Dispatcher.Invoke(DispatcherPriority.Render
                , (Action)(() =>
            {
                this.WorkflowDesigner.DebugManagerView.CurrentLocation = srcLoc;
                
            }));
        
        }

        //Provide Debug Adornment on the Activity being executed
        void textBox1_SelectionChanged(object sender, RoutedEventArgs e)
        {

            string text = this.tx.Text;

            int index = 0;
            int lineClicked = 0;
            while (index < text.Length)
            {
                if (text[index] == '\n')
                    lineClicked++;
                if (this.tx.SelectionStart <= index)
                    break;

                index++;
            }


            this.Dispatcher.Invoke(DispatcherPriority.Normal, (Action)(() =>
            {
                try
                {
                    //Tell Debug Service that the Line Clicked is _______
                    this.WorkflowDesigner.DebugManagerView.CurrentLocation = textLineToSourceLocationMap[lineClicked];
                }
                catch (Exception)
                {
                    //If the user clicks other than on the tracking records themselves.
                    this.WorkflowDesigner.DebugManagerView.CurrentLocation = new SourceLocation(workflowPath, 1, 1, 1, 10);
                }
            }));

        }

      
        private Dictionary<string, Activity> BuildActivityIdToWfElementMap(Dictionary<object, SourceLocation> wfElementToSourceLocationMap)
        {
            Dictionary<string, Activity> map = new Dictionary<string, Activity>();

            Activity wfElement;
            foreach (object instance in wfElementToSourceLocationMap.Keys)
            {
                wfElement = instance as Activity;
                if (wfElement != null)
                {
                    map.Add(wfElement.Id, wfElement);
                }
            }

            return map;
        }

        Dictionary<object, SourceLocation> UpdateSourceLocationMappingInDebuggerService()
        {
            object rootInstance = GetRootInstance();
            Dictionary<object, SourceLocation> sourceLocationMapping = new Dictionary<object, SourceLocation>();
            Dictionary<object, SourceLocation> designerSourceLocationMapping = new Dictionary<object, SourceLocation>();

            if (rootInstance != null)
            {
                Activity documentRootElement = GetRootWorkflowElement(rootInstance);
                SourceLocationProvider.CollectMapping(GetRootRuntimeWorkflowElement(), documentRootElement, sourceLocationMapping,
                    this.WorkflowDesigner.Context.Items.GetValue<WorkflowFileItem>().LoadedFile);
                SourceLocationProvider.CollectMapping(documentRootElement, documentRootElement, designerSourceLocationMapping,
                   this.WorkflowDesigner.Context.Items.GetValue<WorkflowFileItem>().LoadedFile);

                //Activity documentRootElement = GetRootWorkflowElement(rootInstance);
                //SourceLocationProvider.CollectMapping(GetRootRuntimeWorkflowElement(), documentRootElement, sourceLocationMapping,
                //    workflowPath);
                //SourceLocationProvider.CollectMapping(documentRootElement, documentRootElement, designerSourceLocationMapping,
                //   workflowPath);

            }

            // Notify the DebuggerService of the new sourceLocationMapping.
            // When rootInstance == null, it'll just reset the mapping.
            //DebuggerService debuggerService = debuggerService as DebuggerService;
            if (this.DebuggerService != null)
            {
                ((DebuggerService)this.DebuggerService).UpdateSourceLocations(designerSourceLocationMapping);
            }

            return sourceLocationMapping;
        }


        # region Helper Methods
        object GetRootInstance()
        {
            ModelService modelService = this.WorkflowDesigner.Context.Services.GetService<ModelService>();
            if (modelService != null)
            {
                return modelService.Root.GetCurrentValue();
            }
            else
            {
                return null;
            }
        }

        // Get root WorkflowElement.  Currently only handle when the object is ActivitySchemaType or WorkflowElement.
        // May return null if it does not know how to get the root activity.
        Activity GetRootWorkflowElement(object rootModelObject)
        {
            System.Diagnostics.Debug.Assert(rootModelObject != null, "Cannot pass null as rootModelObject");

            Activity rootWorkflowElement;
            IDebuggableWorkflowTree debuggableWorkflowTree = rootModelObject as IDebuggableWorkflowTree;
            if (debuggableWorkflowTree != null)
            {
                rootWorkflowElement = debuggableWorkflowTree.GetWorkflowRoot();
            }
            else // Loose xaml case.
            {
                rootWorkflowElement = rootModelObject as Activity;
            }
            return rootWorkflowElement;
        }

        Activity GetRuntimeExecutionRoot()
         {
            Activity root = deserialize();
            WorkflowInspectionServices.CacheMetadata(root);
    
            return root;
        }

        private Activity deserialize()
        {
            ActivityXamlServicesSettings settings = new ActivityXamlServicesSettings
            {
                CompileExpressions = true
            };

            new XamlXmlReader(workflowPath, new XamlXmlReaderSettings()
            {
                
            });

            return ActivityXamlServices.Load(workflowPath, settings);
        }

        Activity GetRootRuntimeWorkflowElement()
        {
            Activity root = deserialize();
            WorkflowInspectionServices.CacheMetadata(root);
        
            IEnumerator<Activity> enumerator1 = WorkflowInspectionServices.GetActivities(root).GetEnumerator();
            //Get the first child of the x:class
            enumerator1.MoveNext();
            root = enumerator1.Current;
            return root;
        }

        void AddTrackingTextbox()
        {
            tx = new TextBox();
            Grid.SetRow(tx, 1);

            Label trackRecords = new Label();
            trackRecords.FontSize = 11;
            trackRecords.FontWeight = FontWeights.Bold;
            trackRecords.Content = "Tracking Records: ";
            Grid.SetRow(trackRecords, 0);
            this.TrackingRecord.Children.Add(trackRecords);
            this.TrackingRecord.Children.Add(tx);

            //For Tracking Records displayed and to check which activity those records corresponds to.
            this.tx.SelectionChanged += new RoutedEventHandler(textBox1_SelectionChanged);
            textLineToSourceLocationMap = new Dictionary<int, SourceLocation>();

        }
        # endregion
    }
}
