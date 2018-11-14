using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TxFlow.TestWorkflow
{
public abstract class HolidayApprovalActivityToolbox
{
    public virtual System.Boolean CreateHolidayApprovalTaskActivity(
        HolidayApproval.Entities.HolidayRequestEntity HolidayRequest, System.String ResponsibleUser)
    { throw new NotImplementedException("Activity-method has a return value but no mock-implemenation"); }

    public virtual void BookHolidayRequestActivity(
        HolidayApproval.Entities.HolidayRequestEntity HolidayRequest)
    { /* do nothing because activity-method has no return-value */ }


    public virtual void SendEmailActivity(
        System.String Body, System.String Subject, System.String ToMailAddress)
    { /* do nothing because activity-method has no return-value */ }


    public virtual void UpdateHolidayRequestStateActivity(
        HolidayApproval.Entities.HolidayRequestEntity HolidayRequest, HolidayApproval.Entities.EHolidayRequestState NewState)
        { /* do nothing because activity-method has no return-value */ }
    }
}
