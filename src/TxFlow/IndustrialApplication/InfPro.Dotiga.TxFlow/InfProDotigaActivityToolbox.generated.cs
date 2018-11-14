namespace InfPro.Dotiga.TxFlow
{
    public abstract class InfProDotigaActivityToolbox
    {
       public virtual System.String CreateTaskActivity (System.String ResponsibleUser, System.Collections.Generic.Dictionary<System.String,System.Object> ReadonlyTaskFields, System.Collections.Generic.Dictionary<System.String,System.Object> EditableTaskFields, System.Collections.Generic.List<System.String> TaskCommands)
       { throw new System.NotImplementedException("Activity - method has a return value but no mock - implemenation"); }


       public virtual InfPro.Dotiga.ValueObjects.DepotInstanceVO GetDepotInstanceActivity (System.String DepotTypeName, System.Guid DepotInstanceID)
       { throw new System.NotImplementedException("Activity - method has a return value but no mock - implemenation"); }


       public virtual T GetMetaDataValueActivity<T> (InfPro.Dotiga.ValueObjects.DepotInstanceVO DepotInstance, System.String MetaDataFieldName)
       { throw new System.NotImplementedException("Activity - method has a return value but no mock - implemenation"); }


       public virtual System.Collections.Generic.IEnumerable<InfPro.Dotiga.ValueObjects.DocumentVO> SearchDocumentsActivity (InfPro.Dotiga.ValueObjects.DocumentSearchConfigVO SearchConfig)
       { throw new System.NotImplementedException("Activity - method has a return value but no mock - implemenation"); }


       public virtual void SendEmailActivity (System.String Subject, System.String Body, System.String ToMailAddress)
       {  }


       public virtual void SleepActivity (System.TimeSpan TimeSpan)
       {  }


       public virtual void SleepUntilActivity (System.DateTime Until)
       {  }

    }
}