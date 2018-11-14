namespace InfPro.Dotiga.TxFlow
{
    public class PexInfProDotigaActivityToolbox : InfProDotigaActivityToolbox
    {
       public override System.String CreateTaskActivity (System.String ResponsibleUser, System.Collections.Generic.Dictionary<System.String,System.Object> ReadonlyTaskFields, System.Collections.Generic.Dictionary<System.String,System.Object> EditableTaskFields, System.Collections.Generic.List<System.String> TaskCommands)
       {
           return Microsoft.Pex.Framework.PexChoose.Value<System.String>(nameof(CreateTaskActivity));
       }


       public override InfPro.Dotiga.ValueObjects.DepotInstanceVO GetDepotInstanceActivity (System.String DepotTypeName, System.Guid DepotInstanceID)
       {
           return Microsoft.Pex.Framework.PexChoose.Value<InfPro.Dotiga.ValueObjects.DepotInstanceVO>(nameof(GetDepotInstanceActivity));
       }


       public override T GetMetaDataValueActivity<T> (InfPro.Dotiga.ValueObjects.DepotInstanceVO DepotInstance, System.String MetaDataFieldName)
       {
           return Microsoft.Pex.Framework.PexChoose.Value<T>(nameof(GetMetaDataValueActivity));
       }


       public override System.Collections.Generic.IEnumerable<InfPro.Dotiga.ValueObjects.DocumentVO> SearchDocumentsActivity (InfPro.Dotiga.ValueObjects.DocumentSearchConfigVO SearchConfig)
       {
           return Microsoft.Pex.Framework.PexChoose.Value<System.Collections.Generic.IEnumerable<InfPro.Dotiga.ValueObjects.DocumentVO>>(nameof(SearchDocumentsActivity),true,false);
       }

    }
}