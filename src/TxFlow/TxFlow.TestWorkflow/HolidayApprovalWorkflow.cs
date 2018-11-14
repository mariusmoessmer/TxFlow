using HolidayApproval.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static TxFlow.TestWorkflow.HolidayApprovalWorkflow;

namespace TxFlow.TestWorkflow
{
    //public class WorkflowName : TxFlow.CSharpDSL.AbstractWorkflow<ActivityToolboxClassName>
    //{
    //    public TypeOfOutArgument Execute(ListOfArguments)
    //    {
    //        // implemenation of workflow
    //    }
    //}

    public
    class HolidayApprovalWorkflow
    : TxFlow.CSharpDSL.AbstractWorkflow<HolidayApprovalActivityToolbox>
    {
        public void Execute(HolidayRequestEntity HolidayRequest)
        {
            System.Boolean staffManagerDecision;
            staffManagerDecision = this.Activities.
                CreateHolidayApprovalTaskActivity(HolidayRequest, "StaffManager");

            if (staffManagerDecision == false)
            {
                // refused
                this.Activities.UpdateHolidayRequestStateActivity(HolidayRequest,
                    EHolidayRequestState.Refused);
                this.Activities.SendEmailActivity("Holiday request",
                    "Your holiday request has been refused!",
                    HolidayRequest.Originator.EmailAddress);
            }
            else
            {
                // approved
                // implemenation omitted
                #region implementation collapsed
                this.Activities.UpdateHolidayRequestStateActivity(HolidayRequest, EHolidayRequestState.Approved);
                this.Activities.SendEmailActivity("Your holiday request has been approved!", "Holiday request", HolidayRequest.Originator.EmailAddress);
                this.Activities.BookHolidayRequestActivity(HolidayRequest);
                this.Activities.UpdateHolidayRequestStateActivity(HolidayRequest, EHolidayRequestState.Booked);
                #endregion
            }
        }
    }

    //    public
    //class HolidayApprovalWorkflow
    //: TxFlow.CSharpDSL.AbstractWorkflow<HolidayApprovalActivityToolbox>
    //    {
    //        public void Execute(HolidayRequestEntity HolidayRequest)
    //        {
    //            System.Boolean staffManagerDecision;
    //            staffManagerDecision = this.Activities.
    //                CreateHolidayApprovalTaskActivity(HolidayRequest, "StaffManager");

    //            if (staffManagerDecision == false)
    //            {
    //                // refused
    //                this.Activities.UpdateHolidayRequestStateActivity(HolidayRequest,
    //                    EHolidayRequestState.Refused);
    //                this.Activities.SendEmailActivity("Holiday request",
    //                    "Your holiday request has been refused!",
    //                    HolidayRequest.Originator.EmailAddress);
    //            }
    //            else
    //            {
    //                // approved
    //                // implemenation omitted
    //            }
    //        }
    //    }





    //public abstract class HolidayApprovalActivityToolbox
    //{
    //    public abstract bool CreateHolidayApprovalTask(
    //        HolidayRequestEntity HolidayRequest, string ResponsibleUser);

    //    public abstract void BookHolidayRequest(
    //        HolidayRequestEntity HolidayRequest);

    //    public abstract void SendEmail(
    //        string Subject, string Body, string ToMailAddress);

    //    public abstract void UpdateHolidayRequestState(
    //        HolidayRequestEntity HolidayRequest, EHolidayRequestState NewState);


    //}
}
