using HolidayApproval.Entities;

namespace HolidayApprovalWFConsoleApp
{
    class HolidayApprovalWorkflowC
    : AbstractWorkflow<HolidayApprovalActivityToolbox>
    {
        public void Execute(HolidayRequestEntity HolidayRequest)
        {
            System.Boolean staffManagerDecision;
            staffManagerDecision = this.Activities.
                CreateHolidayApprovalTask(HolidayRequest, "StaffManager");

            if (staffManagerDecision == false)
            {
                // refused
                this.Activities.UpdateHolidayRequestState(HolidayRequest, 
                    EHolidayRequestState.Refused);
                this.Activities.SendEmail("Holiday request", 
                    "Your holiday request has been refused!", 
                    HolidayRequest.Originator.EmailAddress);
            }
            else
            {
                // approved
                // implementation omitted
            }
        }
    }
}
