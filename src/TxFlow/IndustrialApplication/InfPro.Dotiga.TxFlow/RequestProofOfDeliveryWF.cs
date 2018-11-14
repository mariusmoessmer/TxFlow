using InfPro.Dotiga.ValueObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using TxFlow.CSharpDSL;

namespace InfPro.Dotiga.TxFlow
{
 public class RequestProofOfDeliveryWF : AbstractWorkflow<InfProDotigaActivityToolbox>
 {
  public void Execute(DepotInstanceVO orderVO)
  {
   DateTime unloadingDateTime = 
    this.Activities.GetMetaDataValueActivity<DateTime>(orderVO, "UnloadingDateTime");
   this.Activities.SleepUntilActivity(unloadingDateTime.AddDays(5));

   bool documentsAvailable = checkIfProofOfDeliveryIsAvailable(orderVO);
   if (!documentsAvailable)
   {
    this.sendRequestEmailToSupplier(orderVO);
    documentsAvailable = dailyCheck(orderVO, 3);
    while (!documentsAvailable)
    {
     sendRequestEmailToSupplier(orderVO);
     documentsAvailable = dailyCheck(orderVO, 2);
     if (!documentsAvailable)
     {
      createTaskForSecretary(orderVO);
      documentsAvailable = dailyCheck(orderVO, 2);
     }
    }
   }
  }

  private bool dailyCheck(DepotInstanceVO orderVO1, int amountOfDays)
  {
   bool documentsAvailableDailyCheck = false;
   for (int i = 0; i < amountOfDays && !documentsAvailableDailyCheck; i++)
   {
    this.Activities.SleepActivity(TimeSpan.FromDays(1));
    documentsAvailableDailyCheck = checkIfProofOfDeliveryIsAvailable(orderVO1);
   }
   return documentsAvailableDailyCheck;
  }

  private void sendRequestEmailToSupplier(DepotInstanceVO orderVO2)
  {
   Guid supplierID = Activities.GetMetaDataValueActivity<Guid>(orderVO2, "SupplierID");
   DepotInstanceVO carrierVO = this.Activities.GetDepotInstanceActivity("Carrier", supplierID);
   string tourNo = this.Activities.GetMetaDataValueActivity<string>(orderVO2, "TourNo");
   string carrierEmail = 
    this.Activities.GetMetaDataValueActivity<string>(carrierVO, "ProofOfDeliveryRequestEmail");

   this.Activities.SendEmailActivity("Proof of delivery for tour " + tourNo,
       "Please send us the proof of delivery - document for tour " + tourNo + 
       Environment.NewLine + "Best regards", carrierEmail);
  }

  private bool checkIfProofOfDeliveryIsAvailable(DepotInstanceVO orderVO3)
  {
   DocumentSearchConfigVO searchConfigVO = new DocumentSearchConfigVO()
   {
    DepotInstance = orderVO3,
    DocumentClass = "ProofOfDelivery",
   };
   var foundDocs = this.Activities.SearchDocumentsActivity(searchConfigVO);
   return foundDocs.Any();
  }

  private void createTaskForSecretary(DepotInstanceVO orderVO5)
  {
   string tourNo = this.Activities.GetMetaDataValueActivity<string>(orderVO5, "TourNo");
   this.Activities.CreateTaskActivity("Administration",
       new Dictionary<string, object>() { { "LabelTourNo", tourNo } },
       new Dictionary<string, object>(),
       new List<string>() { "LabelOK" }
   );
  }
 }
}
