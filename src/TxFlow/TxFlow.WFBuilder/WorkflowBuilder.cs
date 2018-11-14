using Microsoft.VisualBasic.Activities;
using System;
using System.Activities;
using System.Activities.Statements;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TxFlow.WFBuilder.Layout;
using TxFlow.Debug.ValueObjects;
using System.Xml.Serialization;
using System.Text.RegularExpressions;
using System.Activities.XamlIntegration;
using TxFlow.Debug.WFTracking;

namespace TxFlow.WFBuilder
{
    public class WorkflowBuilder : FlowChartBuilder
    {
        private readonly ActivityBuilder _activityBuilder = null;


        public WorkflowBuilder(string workflowName, IEnumerable<Tuple<string, string>> usings) : base(null, workflowName)
        {
            this._workflowName = workflowName;
            this._workflowDefinitionID = Guid.NewGuid().ToString().Replace("-", "");
            _activityBuilder = new ActivityBuilder()
            {
                Implementation = this.FlowChart,
            };

            _activityBuilder.Name = workflowName + "_" + _workflowDefinitionID;


            VisualBasicSettings visualBasicSettings = new VisualBasicSettings();

            IEnumerable<Tuple<string, string>> assemblyAndNameSpaceToImport =
                usings.Where(x=> x.Item2 != "TxFlow.CSharpDSL")
                        .Select(x =>
                        {
                            if (string.IsNullOrEmpty(x.Item1) && x.Item2.StartsWith("System"))
                            {
                                return Tuple.Create("mscorlib", x.Item2);
                            }

                            return x;
                        });


            foreach (var assemblyToImport in assemblyAndNameSpaceToImport)
            {
                // if assembly is not loaded within current domain --> just skip assembly because it is better to be able to open WF-designer without import instead of not opening wf-designer
                if (!AppDomain.CurrentDomain.GetAssemblies().Any(x => x.GetName().Name == assemblyToImport.Item1))
                {
                    continue;
                }


                VisualBasicImportReference visualBasicImportReference = new VisualBasicImportReference();
                visualBasicImportReference.Assembly = assemblyToImport.Item1;
                visualBasicImportReference.Import = assemblyToImport.Item2;
                visualBasicSettings.ImportReferences.Add(visualBasicImportReference);
            }

            VisualBasic.SetSettings(this._activityBuilder, visualBasicSettings);


        }



        private List<Tuple<Activity, ActivitySourceLocationVO>> _debugLocations = new List<Tuple<Activity, ActivitySourceLocationVO>>();
        private string _workflowName;
        private string _workflowDefinitionID;

        public override List<Tuple<Activity, ActivitySourceLocationVO>> DebugLocations
        {
            get
            {
                return _debugLocations;
            }
        }

        public void Validate()
        {
            var activity = WorkflowSerializationHelper.CreateActivity(WorkflowSerializationHelper.ToXaml(_activityBuilder));

            System.Activities.Validation.ValidationResults results = System.Activities.Validation.ActivityValidationServices.Validate(activity);

            
            if(results.Errors.Any())
            {
                throw new Exception("Errors in resulting workflow: " + Environment.NewLine + string.Join(Environment.NewLine, results.Errors.Select(e => "\t" + e)));
            }

            if (results.Warnings.Any())
            {
                throw new Exception("Warnings in resulting workflow: " + Environment.NewLine + string.Join(Environment.NewLine, results.Warnings.Select(e => "\t" + e)));
            }
        }

        private const string DEBUGSYMBOL = "DEBUGSYMBOL";
        private static readonly int DEBUGSYMBOL_LENGTH = DEBUGSYMBOL.Length;
        private static readonly Regex DETECT_DEBUGSYMBOL_REGEX = new Regex("("+DEBUGSYMBOL+".*\\|)", RegexOptions.Multiline | RegexOptions.Compiled);

        private void fillDebugIdActivityIdMapping(Activity parent, Dictionary<int, string> mappingDict)
        {
            if(parent.DisplayName.StartsWith(DEBUGSYMBOL))
            {
                int length = parent.DisplayName.IndexOf('|') - DEBUGSYMBOL_LENGTH;

                int debugId = int.Parse(parent.DisplayName.Substring(DEBUGSYMBOL_LENGTH, length));
                mappingDict[debugId] = parent.Id;
            }

            foreach(var act in WorkflowInspectionServices.GetActivities(parent))
            {
                fillDebugIdActivityIdMapping(act, mappingDict);
            }
        }

        public void SerializeTo(string path)
        {
            var activityBuilder = this._activityBuilder;

            //string tmp = WorkflowSerializationHelper.ToXaml(this._activityBuilder);
            //File.WriteAllText(path + ".ugly.xaml", tmp);

            int debugId = 0;
            this._debugLocations.ForEach(x =>
            {
                x.Item1.DisplayName = DEBUGSYMBOL + (debugId++)+"|" + x.Item1.DisplayName; }
            );


            string xaml = FlowChartLayoutingUtil.ArrangeLayout(activityBuilder);


            var activity = WorkflowSerializationHelper.CreateActivity(xaml);

            var debugIdActivityIdMapping = new Dictionary<int, string>();
            fillDebugIdActivityIdMapping(activity, debugIdActivityIdMapping);


            xaml = DETECT_DEBUGSYMBOL_REGEX.Replace(xaml, string.Empty);

            File.WriteAllText(path, xaml);
            //File.WriteAllText(path, WorkflowSerializationHelper.ToXaml(activityBuilder));


            int count = 0;
            new WorkflowDebugSourceMapVO()
            {
                WorkflowName = this._workflowName,
                WorkflowDefinitionID = this._workflowDefinitionID,
                ActivitySourceLocations = this._debugLocations.Select(x =>
                    {
                        x.Item2.ActivityId = debugIdActivityIdMapping[count++];
                        return x.Item2;
                    }).ToArray()
            }.SerializeTo(Path.GetDirectoryName(path) + "\\" + Path.GetFileNameWithoutExtension(path) + ".txdb");

        }

        public void WithUsings(IEnumerable<string> namespaces)
        {
        }

        public void WithInArguments(params Tuple<string, string>[] args)
        {
            foreach (Tuple<string, string> arg in args)
            {
                string name = arg.Item1;
                string fullTypeName = arg.Item2;

                //Type type = Type.GetType(fullTypeName, true);

                Type type = WFActivities.WFActivityTypeHelper.ResolveType(fullTypeName);


                Type inArgumenttype = typeof(InArgument<>).MakeGenericType(type);
                _activityBuilder.Properties.Add(new DynamicActivityProperty
                {
                    Name = name,
                    Type = inArgumenttype
                });
            }
        }

        public void TestInvoke(IDictionary<string, object> inputs)
        {
            string xaml = WorkflowSerializationHelper.ToXaml(this._activityBuilder);
            WorkflowInvoker.Invoke(WorkflowSerializationHelper.CreateActivity(xaml), inputs);
        }
    }
}
