using System;
using System.Collections.Generic;
using System.Linq;
using InfPro.Dotiga.ValueObjects;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TxFlow.CSharpDSL;

namespace InfPro.Dotiga.TxFlow.ManualTests
{
 [TestClass]
 public class RequestProofOfDeliveryWFTests
 {

[TestMethod]
public void TestPODArrives8DaysAfterUnloadingDate()
{
 var activityToolbox = new TestInfProDotigaActivityToolbox();
 var activityToolboxWithLogging = new InfProDotigaActivityToolboxLogger(activityToolbox);
 var workflow = new RequestProofOfDeliveryWF();
 workflow.RegisterActivityToolbox(activityToolboxWithLogging);
 activityToolbox.NumOfTimesProofOfDeliveryShouldNotExist = 8;
 workflow.Execute(new DepotInstanceVO());

 // filter activity-invocations that are of interest for this unit-test
 IEnumerable<string> activityInvocationsOfInterest = activityToolboxWithLogging
     .ActivityInvocationLog.ActivityInvocations.Where(x =>
         x.ActivityName == nameof(InfProDotigaActivityToolbox.SleepUntilActivity)
         || x.ActivityName == nameof(InfProDotigaActivityToolbox.SleepActivity)
         || x.ActivityName == nameof(InfProDotigaActivityToolbox.SendEmailActivity)
         || x.ActivityName == nameof(InfProDotigaActivityToolbox.CreateTaskActivity))
     .Select(x => x.ActivityName);

 // create list of expected activity-invocations
 IEnumerable<string> expectedActivityInvocations = new[] {
   // Wait 5 days after transport's unloading date
   nameof(InfProDotigaActivityToolbox.SleepUntilActivity),
   // send request e-mail to carrier
   nameof(InfProDotigaActivityToolbox.SendEmailActivity),
   // Check every day, if proof of delivery is archived in order-depot, for 3 days
   nameof(InfProDotigaActivityToolbox.SleepActivity),
   nameof(InfProDotigaActivityToolbox.SleepActivity),
   nameof(InfProDotigaActivityToolbox.SleepActivity),
   // first iteration of loop
   nameof(InfProDotigaActivityToolbox.SendEmailActivity),
   nameof(InfProDotigaActivityToolbox.SleepActivity),
   nameof(InfProDotigaActivityToolbox.SleepActivity),
   nameof(InfProDotigaActivityToolbox.CreateTaskActivity),
   nameof(InfProDotigaActivityToolbox.SleepActivity),
   nameof(InfProDotigaActivityToolbox.SleepActivity),
   // second loop iteration
   nameof(InfProDotigaActivityToolbox.SendEmailActivity),
   nameof(InfProDotigaActivityToolbox.SleepActivity)
 };
 Assert.IsTrue(activityInvocationsOfInterest.SequenceEqual(expectedActivityInvocations));
}

  [DataRow(0, 0, 0)]
  [DataRow(1, 1, 0)]
  [DataRow(2, 1, 0)]
  [DataRow(3, 1, 0)]

  [DataRow(4, 2, 0)]
  [DataRow(5, 2, 0)]
  [DataRow(6, 2, 1)]
  [DataRow(7, 2, 1)]
  [DataRow(8, 3, 1)]
  [DataRow(9, 3, 1)]
  [DataRow(10, 3, 2)]
  [DataRow(11, 3, 2)]
  [DataRow(12, 4, 2)]
  [DataRow(13, 4, 2)]
  [DataTestMethod]
  public void TestPODArrivesXDaysAfterUnloadingDate(int days, int amountOfEmails, int amountOfTasksCreated)
  {
   var activityToolbox = new TestInfProDotigaActivityToolbox();
   var activityToolboxWithLogging = new InfProDotigaActivityToolboxLogger(activityToolbox);
   var workflow = new RequestProofOfDeliveryWF();
   workflow.RegisterActivityToolbox(activityToolboxWithLogging);
   activityToolbox.NumOfTimesProofOfDeliveryShouldNotExist = days;
   workflow.Execute(new DepotInstanceVO());

   // check if workflow waits for unloading-date
   Assert.AreEqual(1, activityToolboxWithLogging.ActivityInvocationLog
    .ActivityInvocations.Count(x => x.ActivityName == nameof(InfProDotigaActivityToolbox.SleepUntilActivity)));
   // check if workflow waits number of days as expected
   Assert.AreEqual(days, activityToolboxWithLogging.ActivityInvocationLog
   .ActivityInvocations.Count(x => x.ActivityName == nameof(InfProDotigaActivityToolbox.SleepActivity)));
   // check if workflow has sent correct number of e-mails
   Assert.AreEqual(amountOfEmails, activityToolboxWithLogging.ActivityInvocationLog
    .ActivityInvocations.Count(x => x.ActivityName == nameof(InfProDotigaActivityToolbox.SendEmailActivity)));
   // check if workflow has immediately stopped after proof of delivery - document was archived
   Assert.IsTrue(activityToolboxWithLogging.ActivityInvocationLog.ActivityInvocations.Last()
    .ActivityName == nameof(InfProDotigaActivityToolbox.SearchDocumentsActivity)
   );

   // check if workflow has created correct number of tasks
   Assert.AreEqual(amountOfTasksCreated, activityToolboxWithLogging.ActivityInvocationLog
    .ActivityInvocations.Count(x => x.ActivityName == nameof(InfProDotigaActivityToolbox.CreateTaskActivity
   )));
  }

  private class TestInfProDotigaActivityToolbox : InfProDotigaActivityToolbox
  {
   public int NumOfTimesProofOfDeliveryShouldNotExist { get; set; }

   public override T GetMetaDataValueActivity<T>(DepotInstanceVO DepotInstance, string MetaDataFieldName)
   {
    return default(T);
   }

   public override IEnumerable<DocumentVO> SearchDocumentsActivity(DocumentSearchConfigVO SearchConfig)
   {
    if (this.NumOfTimesProofOfDeliveryShouldNotExist <= 0)
    {
     return new DocumentVO[] { new DocumentVO() };
    }
    this.NumOfTimesProofOfDeliveryShouldNotExist--;
    return new DocumentVO[] { };
   }

   public override DepotInstanceVO GetDepotInstanceActivity(string DepotTypeName, Guid DepotInstanceID)
   {
    return default(DepotInstanceVO);
   }

   public override string CreateTaskActivity(string ResponsibleUser, Dictionary<string, object> ReadonlyTaskFields, Dictionary<string, object> EditableTaskFields, List<string> TaskCommands)
   {
    return default(string);
   }
  }
 }
}
