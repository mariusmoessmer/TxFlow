using System;
using System.Diagnostics;
using HolidayApproval.Entities;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace TxFlow.TestWorkflow.Tests
{
    [TestClass]
    public class HolidayApprovalTest
    {
        private 
class TestHolidayApprovalActivityToolbox
    : HolidayApprovalActivityToolbox
{
    public bool HolidayApprovalTaskCreated = false;
    public override bool CreateHolidayApprovalTaskActivity(
        HolidayRequestEntity HolidayRequest, string ResponsibleUser)
    {
        HolidayApprovalTaskCreated = true;
        return false; // simulate refusal by returning false
    }

    public bool HolidayRequestStateUpdated = false;
    public override void UpdateHolidayRequestStateActivity(
        HolidayRequestEntity HolidayRequest, EHolidayRequestState NewState)
    { HolidayRequestStateUpdated = true; }

    public bool EmailSent = false;
    public override void SendEmailActivity(
        string Body, string Subject, string ToMailAddress)
    { EmailSent = true; }

    public override void BookHolidayRequestActivity(
        HolidayRequestEntity HolidayRequest)
    { throw new Exception("Holiday Request must not be booked in case of refusal!"); }
}


[TestMethod]
public void TestRefusalOfHolidayRequest()
{
// create activitytoolbox for testing
var activityToolboxForTesting = new TestHolidayApprovalActivityToolbox();
            // instantiate workflow
var workflow = new HolidayApprovalWorkflow();
            workflow.RegisterActivityToolbox(activityToolboxForTesting);
// execute workflow
workflow.Execute(new HolidayRequestEntity(
    holidayFrom: new DateTime(2018, 12, 12),
    holidayTo: new DateTime(2018, 12, 23),
    originator: new UserEntity("Marius", "marius@domain.xyz")
)); 

// check assertions
Assert.IsTrue(activityToolboxForTesting.HolidayApprovalTaskCreated);
Assert.IsTrue(activityToolboxForTesting.HolidayRequestStateUpdated);
Assert.IsTrue(activityToolboxForTesting.EmailSent);
}

private void forThesis()
{
    // create objects which are used to start workflow
    var initialHolidayRequest = new HolidayRequestEntity(
        holidayFrom: new DateTime(2018, 12, 12),
        holidayTo: new DateTime(2018, 12, 23),
        originator: new UserEntity("Marius", "marius@domain.xyz")
    );

    // create instance of workflow
    var workflow = new HolidayApprovalWorkflow();
    // run workflow
    workflow.Execute(initialHolidayRequest);
    // TODO: check assertions
}
    }
}
