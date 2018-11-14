using System;
using System.Collections.Generic;
using System.Linq;
using Autofac;
using Autofac.Extras.DynamicProxy;
using HolidayApproval.Entities;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TxFlow.TestWorkflow;

namespace TestInterception
{
    [TestClass]
    public class UnitTest1
    {
public class TestHolidayApprovalActivityToolbox
    : HolidayApprovalActivityToolbox
{

    public override bool CreateHolidayApprovalTaskActivity(
        HolidayRequestEntity HolidayRequest, string ResponsibleUser)
    { return false; /* simulate refusal by returning false */ }

    // implementation of other activity-methods omitted
}

        [TestMethod]
public void TestRefusalOfHolidayRequest()
{
var activityToolbox = new TestHolidayApprovalActivityToolbox();
var activityToolboxWithLogging = new HolidayApprovalActivityToolboxLogger(activityToolbox);
var workflow = new HolidayApprovalWorkflow();
workflow.RegisterActivityToolbox(activityToolboxWithLogging);
workflow.Execute(new HolidayRequestEntity(
    holidayFrom: new DateTime(2018, 12, 12),
    holidayTo: new DateTime(2018, 12, 23),
    originator: new UserEntity("Marius", "marius@domain.xyz")
));

Assert.AreEqual(nameof(TestHolidayApprovalActivityToolbox.CreateHolidayApprovalTaskActivity),
    activityToolboxWithLogging.ActivityInvocationLog[0].ActivityName);
Assert.AreEqual(nameof(TestHolidayApprovalActivityToolbox.UpdateHolidayRequestStateActivity),
    activityToolboxWithLogging.ActivityInvocationLog[1].ActivityName);
Assert.AreEqual(EHolidayRequestState.Refused,
    activityToolboxWithLogging.ActivityInvocationLog[1].ParameterValuesByName["NewState"]);
Assert.AreEqual(nameof(TestHolidayApprovalActivityToolbox.SendEmailActivity),
    activityToolboxWithLogging.ActivityInvocationLog[2].ActivityName);
}


        [TestMethod]
        public void TestMethod1()
        {
            WorkflowTestExecutionTracer wfTracer;
            var tmp = create<HolidayApprovalActivityToolbox>(out wfTracer);
            tmp.SendEmailActivity("body", "subject", "to");

            var request = new HolidayRequestEntity();
            request.State = EHolidayRequestState.Approved;
            tmp.BookHolidayRequestActivity(request);
            request.State = EHolidayRequestState.New;

            //Assert.IsTrue(wfTracer.ActivityInvocations.Any(x=>x.Method.Nam))
        }

        private T create<T>(out WorkflowTestExecutionTracer wfTracer)
        {
            wfTracer = new WorkflowTestExecutionTracer();
            var wfTracerCopy = wfTracer;
            var b = new ContainerBuilder();
            b.Register(i => wfTracerCopy);
            b.RegisterType<T>().EnableClassInterceptors().InterceptedBy(typeof(WorkflowTestExecutionTracer));
            var container = b.Build();
            return container.Resolve<T>();
        }
    }
}
