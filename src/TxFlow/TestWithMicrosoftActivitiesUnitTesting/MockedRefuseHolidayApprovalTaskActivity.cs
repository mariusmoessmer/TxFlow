using HolidayApproval.Entities;
using HolidayApprovalWFConsoleApp;
using System.Activities;

namespace TestWithMicrosoftActivitiesUnitTesting
{
    public class MockedRefuseHolidayApprovalTaskActivity : NativeActivity
    {
        public MockedRefuseHolidayApprovalTaskActivity()
        {
        }

        public InArgument<string> ResponsibleUser { get; set; }

        public OutArgument<System.Boolean> Result { get; set; }
        public InArgument<HolidayRequestEntity> HolidayRequest { get; set; }

        protected override void Execute(NativeActivityContext context)
        {
            this.Result.Set(context, false);
        }
    }
}