//----------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
//----------------------------------------------------------------
using System;
using System.Activities;
using System.Activities.Tracking;
using System.Collections.Generic;
using System.Linq;


namespace Microsoft.Samples.VisualWorkflowTracking
{
    public class VisualTrackingParticipant : System.Activities.Tracking.TrackingParticipant
    {
        const String all = "*";


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

            foreach(var activity in WorkflowInspectionServices.GetActivities(root))
            {
                var propVars = activity.GetType().GetProperties().FirstOrDefault(p => typeof(IEnumerable<Variable>).IsAssignableFrom(p.PropertyType));
                if (propVars != null)
                {
                    variables.AddRange(((IEnumerable<Variable>)propVars.GetValue(activity, null)).Select(x=>x.Name));
                }

                collectVariablesActivity(activity, variables);
            }
        }



        public VisualTrackingParticipant(Activity root)
        {
            List<string> arguments = root is DynamicActivity ? getArgumentsNames(root as DynamicActivity) : new List<string>();
            List<string> variables = new List<string>();
            collectVariablesActivity(root, variables);


            var activityStateQuery = new ActivityStateQuery()
            {
                ActivityName = all,
                States = { all },
            };

            arguments.ForEach(x => activityStateQuery.Arguments.Add(x));
            variables.ForEach(x => activityStateQuery.Variables.Add(x));


            TrackingProfile = new TrackingProfile()
            {
                Name = "CustomTrackingProfile",
                Queries =
                        {
                            new CustomTrackingQuery()
                            {
                                Name = all,
                                ActivityName = all
                            },
                            new WorkflowInstanceQuery()
                            {
                                // Limit workflow instance tracking records for started and completed workflow states
                                States = { WorkflowInstanceStates.Started, WorkflowInstanceStates.Completed },
                            },
                            new ActivityScheduledQuery()
                            {
                                ActivityName = all,
                                ChildActivityName = all,
                            },
                            activityStateQuery
                        }
            };
        }


        public event EventHandler<TrackingEventArgs> TrackingRecordReceived;

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

            internal IDictionary<string, object> GetVariables(string activityId)
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

        private Dictionary<Guid, WorkflowVariableValues> _lastVariablesPerWorkflowInstance = new Dictionary<Guid, WorkflowVariableValues>();

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

        protected override void Track(TrackingRecord record, TimeSpan timeout)
        {
            System.Diagnostics.Debug.WriteLine(
                String.Format("Tracking Record Received: {0}", record)
            );

            if (TrackingRecordReceived != null)
            {
                ActivityStateRecord activityExecuted = record as ActivityStateRecord;

                ActivityScheduledRecord activityScheduled = record as ActivityScheduledRecord;

                if (activityExecuted != null)
                {
                    Dictionary<string, object> variables = new Dictionary<string, object>(activityExecuted.Variables);
                    //foreach(var kvp in activityExecuted.Arguments)
                    //{
                    //    variables[kvp.Key] = kvp.Value;
                    //}


                    getOrAddWorkflowVariables(activityExecuted.InstanceId).ActivityExecuted(activityExecuted.Activity.Id, variables);
                }
                else if (activityScheduled != null)
                {
                    var workflowVariables = getOrAddWorkflowVariables(activityScheduled.InstanceId);

                    string parentActivityId = activityScheduled.Activity != null ? activityScheduled.Activity.Id : null;
                    workflowVariables.ActivityScheduled(activityScheduled.Child.Id, parentActivityId);


                    IDictionary<string, object> vars = parentActivityId != null ? workflowVariables.GetVariables(parentActivityId) : new Dictionary<string, object>();

                    TrackingRecordReceived(this, new TrackingEventArgs(record, activityScheduled.Child.Id, vars));
                }


            }
        }
    }

    //Custom Tracking EventArgs
    public class TrackingEventArgs : EventArgs
    {
        public TrackingRecord Record { get; }
        public IDictionary<string, object> Variables { get; }
        public string ActivityId { get; }

        public TrackingEventArgs(TrackingRecord trackingRecord, string activityId, IDictionary<string, object> variables)
        {
            this.Record = trackingRecord;
            this.ActivityId = activityId;
            this.Variables = variables;
        }
    }
}
