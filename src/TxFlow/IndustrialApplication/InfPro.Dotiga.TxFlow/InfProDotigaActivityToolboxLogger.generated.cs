using TxFlow.CSharpDSL;

namespace InfPro.Dotiga.TxFlow
{
    public class InfProDotigaActivityToolboxLogger : InfProDotigaActivityToolbox
    {
       private readonly InfProDotigaActivityToolbox _realObject;
       private readonly ActivityInvocationLog _log = new ActivityInvocationLog();
       public InfProDotigaActivityToolboxLogger(InfProDotigaActivityToolbox realObject)
       {
           _realObject = realObject;
       }

       public ActivityInvocationLog ActivityInvocationLog => _log;
       public override System.String CreateTaskActivity (System.String ResponsibleUser, System.Collections.Generic.Dictionary<System.String,System.Object> ReadonlyTaskFields, System.Collections.Generic.Dictionary<System.String,System.Object> EditableTaskFields, System.Collections.Generic.List<System.String> TaskCommands)
       {
           System.String returnValue = _realObject.CreateTaskActivity(ResponsibleUser, ReadonlyTaskFields, EditableTaskFields, TaskCommands);
           _log.Log(new ActivityInvocation(nameof(CreateTaskActivity),
                     new System.Collections.Generic.Dictionary<string, object>() { { nameof(ResponsibleUser), ResponsibleUser}, { nameof(ReadonlyTaskFields), ReadonlyTaskFields}, { nameof(EditableTaskFields), EditableTaskFields}, { nameof(TaskCommands), TaskCommands} },
                     returnValue));
           return returnValue;
       }


       public override InfPro.Dotiga.ValueObjects.DepotInstanceVO GetDepotInstanceActivity (System.String DepotTypeName, System.Guid DepotInstanceID)
       {
           InfPro.Dotiga.ValueObjects.DepotInstanceVO returnValue = _realObject.GetDepotInstanceActivity(DepotTypeName, DepotInstanceID);
           _log.Log(new ActivityInvocation(nameof(GetDepotInstanceActivity),
                     new System.Collections.Generic.Dictionary<string, object>() { { nameof(DepotTypeName), DepotTypeName}, { nameof(DepotInstanceID), DepotInstanceID} },
                     returnValue));
           return returnValue;
       }


       public override T GetMetaDataValueActivity<T> (InfPro.Dotiga.ValueObjects.DepotInstanceVO DepotInstance, System.String MetaDataFieldName)
       {
           T returnValue = _realObject.GetMetaDataValueActivity<T>(DepotInstance, MetaDataFieldName);
           _log.Log(new ActivityInvocation(nameof(GetMetaDataValueActivity),
                     new System.Collections.Generic.Dictionary<string, object>() { { nameof(DepotInstance), DepotInstance}, { nameof(MetaDataFieldName), MetaDataFieldName} },
                     returnValue));
           return returnValue;
       }


       public override System.Collections.Generic.IEnumerable<InfPro.Dotiga.ValueObjects.DocumentVO> SearchDocumentsActivity (InfPro.Dotiga.ValueObjects.DocumentSearchConfigVO SearchConfig)
       {
           System.Collections.Generic.IEnumerable<InfPro.Dotiga.ValueObjects.DocumentVO> returnValue = _realObject.SearchDocumentsActivity(SearchConfig);
           _log.Log(new ActivityInvocation(nameof(SearchDocumentsActivity),
                     new System.Collections.Generic.Dictionary<string, object>() { { nameof(SearchConfig), SearchConfig} },
                     returnValue));
           return returnValue;
       }


       public override void SendEmailActivity (System.String Subject, System.String Body, System.String ToMailAddress)
       {
           _realObject.SendEmailActivity(Subject, Body, ToMailAddress);
           _log.Log(new ActivityInvocation(nameof(SendEmailActivity),
                     new System.Collections.Generic.Dictionary<string, object>() { { nameof(Subject), Subject}, { nameof(Body), Body}, { nameof(ToMailAddress), ToMailAddress} },
                     typeof(void)));
       }


       public override void SleepActivity (System.TimeSpan TimeSpan)
       {
           _realObject.SleepActivity(TimeSpan);
           _log.Log(new ActivityInvocation(nameof(SleepActivity),
                     new System.Collections.Generic.Dictionary<string, object>() { { nameof(TimeSpan), TimeSpan} },
                     typeof(void)));
       }


       public override void SleepUntilActivity (System.DateTime Until)
       {
           _realObject.SleepUntilActivity(Until);
           _log.Log(new ActivityInvocation(nameof(SleepUntilActivity),
                     new System.Collections.Generic.Dictionary<string, object>() { { nameof(Until), Until} },
                     typeof(void)));
       }

    }
}