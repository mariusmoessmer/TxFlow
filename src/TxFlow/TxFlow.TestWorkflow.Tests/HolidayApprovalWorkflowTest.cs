using HolidayApproval.Entities;
// <copyright file="HolidayApprovalWorkflowTest.cs">Copyright ©  2017</copyright>

using System;
using Microsoft.Pex.Framework;
using Microsoft.Pex.Framework.Validation;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TxFlow.TestWorkflow;
using TestInterception;
using System.Linq;
using System.IO;

namespace TxFlow.TestWorkflow.Tests
{
    [TestClass]
    [PexClass(typeof(HolidayApprovalActivityToolbox))]
    [PexAllowedExceptionFromTypeUnderTest(typeof(ArgumentException), AcceptExceptionSubtypes = true)]
    [PexAllowedExceptionFromTypeUnderTest(typeof(InvalidOperationException))]
    public partial class HolidayApprovalWorkflowTest
    {
        public class PexHolidayApprovalToolbox : HolidayApprovalActivityToolbox
        {
            public override bool CreateHolidayApprovalTaskActivity(
                HolidayRequestEntity HolidayRequest, string ResponsibleUser)
            {
                bool chosen = PexChoose.Value<bool>("CreateHolidayApprovalTaskActivityReturnValue");
                PexObserve.ValueForViewing("CreateHolidayApprovalTaskActivityReturnValue", chosen);
                return chosen;
            }
        }

[PexMethod]
public void ExploreHolidayApprovalWorkflow(
    [PexAssumeNotNull] HolidayRequestEntity holidayRequestEntity)
{
    PexAssume.IsNotNull(holidayRequestEntity.Originator);

    var activityToolboxLogger = new HolidayApprovalActivityToolboxLogger(
        new PexHolidayApprovalToolbox());
    var workflow = new HolidayApprovalWorkflow();
    workflow.RegisterActivityToolbox(activityToolboxLogger);

    workflow.Execute(holidayRequestEntity);

PexSymbolicValue.IgnoreComputation(() =>
{
    string tmpFilePath = System.IO.Path.GetTempPath() 
        + $"ActivityLog_{DateTime.Now.Ticks}.json";
    activityToolboxLogger.ActivityInvocationLog.WriteAsJson(tmpFilePath);
    PexObserve.ValueForViewing("ActivityInvocations", tmpFilePath);
});
}

            

            //PexObserve.ValueForViewing("ActivityInvocationLog", lines);

            //PexObserve.ValueAtEndOfTest("AssertActivityInvocations", activityToolboxLogger.ActivityInvocationLog.ActivityInvocations.Count());

            //int i = 0;
            //foreach(var activityInv in activityToolboxLogger.ActivityInvocationLog.ActivityInvocations)
            //{
            //    PexObserve.ValueAtEndOfTest("activityInv"+ i++, activityInv);
            //}
            ////PexObserve.ValueForViewing("ActivityInvocations", filePath);
        //}

        //[PexMethod]
        //public void ExecuteMariusInterception([PexAssumeNotNull] HolidayRequestEntity holidayRequestEntity)
        //{
        //    WorkflowTestExecutor<HolidayApprovalWorkflow, PexHolidayApprovalToolbox> testExecutor
        //        = new WorkflowTestExecutor<HolidayApprovalWorkflow, PexHolidayApprovalToolbox>();

        //    PexAssume.IsNotNull(holidayRequestEntity.Originator);
        //    testExecutor.Workflow.Execute(holidayRequestEntity);
        //}

        //[PexMethod]
        //public void ExecuteMariusYeah([PexAssumeNotNull]HolidayRequestEntity HolidayRequest)
        //{
        //    PexAssume.IsNotNull(HolidayRequest.Originator);
        //    var wf = new HolidayApprovalWorkflow()
        //    {
        //        Activities = new ImplementedHolidayApprovalToolbox()
        //    };
        //    wf.Execute(HolidayRequest);
        //}
    }
}
