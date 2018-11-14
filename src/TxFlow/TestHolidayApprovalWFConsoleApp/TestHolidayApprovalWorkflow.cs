using System;
using System.Activities;
using System.Collections.Generic;
using HolidayApproval.Entities;
using HolidayApprovalWFConsoleApp;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace TestHolidayApprovalWFConsoleApp
{
    [TestClass]
    public class TestHolidayApprovalWorkflow
    {
[TestMethod]
public void TestUpdateHolidayRequestStateOnRefusal()
{
    // create objects which are used to start workflow
    var initialOriginator = new UserEntity("Marius", "marius@domain.xyz");
    var initialHolidayRequest = new HolidayRequestEntity(
        new DateTime(2018, 12, 12), 
        new DateTime(2018, 12, 23), 
        initialOriginator
    );

    //// register mock-implementation to simulate staff manager's behavior
    //CreateHolidayApprovalTaskActivity.MockImplementationForTesting 
    //    = (HolidayRequestEntity HolidayRequest, string ResponsibleUser) =>
    //{
    //    // false indicates that staff manager does NOT accept holiday approval
    //    return false; 
    //};

    //// provide a mock implemenation for UpdateHolidayRequestStateActivity 
    //// to assert if workflow really updates holiday approval's state to 'Refused'
    //UpdateHolidayRequestStateActivity.MockImplementationForTesting
    //    = (HolidayRequestEntity HolidayRequest, EHolidayRequestState NewState) =>
    //{
    //    // check if workflow provides same HolidayRequest-object
    //    Assert.AreSame(initialHolidayRequest, HolidayRequest);
    //    // check if NewState provided by workflow has value Refused
    //    Assert.AreEqual(EHolidayRequestState.Refused, NewState);
    //};
           
    //WorkflowInvoker.Invoke(
    //    new HolidayApprovalWorkflow(), 
    //    new Dictionary<string, object>() {{ "HolidayRequest", initialHolidayRequest }}
    //);
}
    }
}
