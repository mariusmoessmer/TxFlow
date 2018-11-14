using ChannelAdam.ServiceModel;
using Newtonsoft.Json;
using System;
using System.Activities;
using System.Activities.Tracking;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using TxFlow.Debug.WFTracking.TxFlowDebugServiceReference;

namespace TxFlow.Debug.WFTracking
{
    public class DebugTrackingParticipant : System.Activities.Tracking.TrackingParticipant
    {
        private const string ALL = "*";
        private readonly Action<Exception> _exceptionHandler;
        private readonly string _workflowDebugServiceUri;
        private Dictionary<Guid, WorkflowVariableValues> _lastVariablesPerWorkflowInstance = new Dictionary<Guid, WorkflowVariableValues>();
        private string _workflowDefinitionID;

        public DebugTrackingParticipant(Activity root, string workflowDebugServiceUri = "http://localhost:8071", Action<Exception> exceptionHandler = null)
        {
            _workflowDebugServiceUri = workflowDebugServiceUri;
            _exceptionHandler = exceptionHandler;

            var activityStateQuery = new ActivityStateQuery()
            {
                ActivityName = ALL,
                States = { ALL },
            };

            string wfName;
            List<string> arguments;
            if (root is DynamicActivity)
            {
                var rootDyn = root as DynamicActivity;
                arguments = getArgumentsNames(rootDyn);
                wfName = rootDyn.Name;
            }
            else
            {
                arguments = new List<string>();
                wfName = root.DisplayName;
            }

            string[] splitted = wfName.Split('_');
            if(splitted.Length > 1)
            {
                // it is a valid worlkflow
                this._workflowDefinitionID = splitted.Last();

                List<string> variables = new List<string>();
                collectVariablesActivity(root, variables);

                arguments.ForEach(x => activityStateQuery.Arguments.Add(x));
                variables.ForEach(x => activityStateQuery.Variables.Add(x));

                TrackingProfile = new TrackingProfile()
                {
                    Name = "CustomTrackingProfile",
                    Queries =
                        {
                            new CustomTrackingQuery()
                            {
                                Name = ALL,
                                ActivityName = ALL
                            },
                            new WorkflowInstanceQuery()
                            {
                                // Limit workflow instance tracking records for started and completed workflow states
                                States = { WorkflowInstanceStates.Started, WorkflowInstanceStates.Completed },
                            },
                            new ActivityScheduledQuery()
                            {
                                ActivityName = ALL,
                                ChildActivityName = ALL,
                            },
                            activityStateQuery
                        }
                };
            }

            
        }


        protected override void Track(TrackingRecord record, TimeSpan timeout)
        {
            System.Diagnostics.Debug.WriteLine(
                String.Format("Tracking Record Received: {0}", record)
            );


            WorkflowInstanceRecord workflowInstanceRecord = record as WorkflowInstanceRecord;
            ActivityStateRecord activityExecuted = record as ActivityStateRecord;
            ActivityScheduledRecord activityScheduled = record as ActivityScheduledRecord;

            if(workflowInstanceRecord != null && workflowInstanceRecord.State == "Completed")
            {
                // notify about completion
                notifyDebugAdapter(workflowInstanceRecord.InstanceId.ToString(), "-1", null);
            }else if (activityExecuted != null)
            {
                //object test = TxFlowDebugSymbol.GetWorkflowDefinitionID(activityExecuted.Activity);

                Dictionary<string, object> variables = new Dictionary<string, object>(activityExecuted.Variables);
                //foreach(var kvp in activityExecuted.Arguments)
                //{
                //    variables[kvp.Key] = kvp.Value;
                //}

                getOrAddWorkflowVariables(activityExecuted.InstanceId).ActivityExecuted(activityExecuted.Activity.Id, variables);
            }
            else if (activityScheduled != null)
            {
                var test = activityScheduled.Child.Name;
                //var test = TxFlowDebugSymbol.GetWorkflowDefinitionID(activityScheduled.Child);

                var workflowVariables = getOrAddWorkflowVariables(activityScheduled.InstanceId);

                string parentActivityId = activityScheduled.Activity != null ? activityScheduled.Activity.Id : null;
                workflowVariables.ActivityScheduled(activityScheduled.Child.Id, parentActivityId);


                Dictionary<string, object> vars = parentActivityId != null ? workflowVariables.GetVariables(parentActivityId) : new Dictionary<string, object>();
                notifyDebugAdapter(activityScheduled.InstanceId.ToString(), activityScheduled.Child.Id, vars);
            }
        }

        private void notifyDebugAdapter(string workflowInstanceId, string activityId, Dictionary<string, object> vars)
        {
            try
            {
                using (var client = ServiceConsumerFactory.Create<IWorkflowDebugService>(() =>
                {
                    return new WorkflowDebugServiceClient(new WSHttpBinding()
                    {
                        SendTimeout = TimeSpan.FromHours(1),
                        ReceiveTimeout = TimeSpan.FromHours(1),
                    }, new EndpointAddress(_workflowDebugServiceUri));
                }))
                {
                    var varsToSend = vars != null ? vars.ToDictionary(x => x.Key, x =>
                    {
                        if (x.Value == null)
                        {
                            return null;
                        }

                        if (x.Value.GetType().IsPrimitive)
                        {
                            return x.Value;
                        }

                        StringBuilder sb = new StringBuilder();
                        StringWriter sw = new StringWriter(sb);

                        using (JsonWriter textWriter = new JsonTextWriter(sw))
                        {
                            var serializer = new JsonSerializer();
                            serializer.Serialize(textWriter, x.Value);
                        }

                        return sb.ToString();
                    }) : null;

                    client.Operations.ActivityExecuted(this._workflowDefinitionID, workflowInstanceId, activityId, varsToSend);
                }
            }
            catch (Exception ex)
            {
                var exceptionHandler = this._exceptionHandler;
                if (exceptionHandler != null)
                {
                    exceptionHandler(ex);
                }
                else
                {
                    // simply rethrow
                    throw;
                }

            }
        }

        private List<string> getArgumentsNames(DynamicActivity act)
        {
            List<string> names = new List<string>();
            if (act != null)
            {
                act.Properties.Where(p => typeof(Argument).IsAssignableFrom(p.Type)).ToList().ForEach(dp =>
                {
                    names.Add(dp.Name);
                });

            }

            return names;
        }

        private void collectVariablesActivity(Activity root, List<string> variables)
        {
            foreach (var activity in WorkflowInspectionServices.GetActivities(root))
            {
                var propVars = activity.GetType().GetProperties().FirstOrDefault(p => typeof(IEnumerable<Variable>).IsAssignableFrom(p.PropertyType));
                if (propVars != null)
                {
                    variables.AddRange(((IEnumerable<Variable>)propVars.GetValue(activity, null)).Select(x => x.Name));
                }

                collectVariablesActivity(activity, variables);
            }
        }

        WorkflowVariableValues getOrAddWorkflowVariables(Guid workflowInstanceID)
        {
            WorkflowVariableValues result;
            if (!_lastVariablesPerWorkflowInstance.TryGetValue(workflowInstanceID, out result))
            {
                result = new WorkflowVariableValues();
                _lastVariablesPerWorkflowInstance[workflowInstanceID] = result;
            }

            return result;
        }

        private class WorkflowVariableValues
        {

            private Dictionary<string, Dictionary<string, object>> _activityVariables = new Dictionary<string, Dictionary<string, object>>();
            private Dictionary<string, string> _childParentMapping = new Dictionary<string, string>();


            internal void ActivityExecuted(string id, Dictionary<string, object> variables)
            {
                string parentId = id;
                while ((parentId = _childParentMapping[parentId]) != null)
                {
                    List<KeyValuePair<string, object>> variablesToMerge = new List<KeyValuePair<string, object>>();

                    foreach (var kvp in _activityVariables[parentId])
                    {
                        if (variables.ContainsKey(kvp.Key))
                        {
                            variablesToMerge.Add(new KeyValuePair<string, object>(kvp.Key, variables[kvp.Key]));

                            variables.Remove(kvp.Key);
                        }
                    }
                    foreach (var merge in variablesToMerge)
                    {
                        _activityVariables[parentId][merge.Key] = merge.Value;
                    }
                }

                _activityVariables[id] = variables;
            }

            internal Dictionary<string, object> GetVariables(string activityId)
            {
                Dictionary<string, object> result = new Dictionary<string, object>();


                Dictionary<string, object> current;
                while (activityId != null && _activityVariables.TryGetValue(activityId, out current))
                {
                    foreach (var kvp in current)
                    {
                        result.Add(kvp.Key, kvp.Value);
                    }

                    activityId = _childParentMapping.ContainsKey(activityId) ? _childParentMapping[activityId] : null;

                }

                return result;
            }

            internal void ActivityScheduled(string childActivityId, string parentActivityId)
            {
                _childParentMapping[childActivityId] = parentActivityId;
            }
        }
        
    }
}
