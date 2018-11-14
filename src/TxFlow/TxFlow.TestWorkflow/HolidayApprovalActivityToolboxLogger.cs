using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HolidayApproval.Entities;
using TxFlow.CSharpDSL;

namespace TxFlow.TestWorkflow
{
    public class HolidayApprovalActivityToolboxLogger : HolidayApprovalActivityToolbox
    {
        private readonly HolidayApprovalActivityToolbox _realObject;
        private readonly ActivityInvocationLog _log = new ActivityInvocationLog();

        public HolidayApprovalActivityToolboxLogger(HolidayApprovalActivityToolbox realObject)
        {
            _realObject = realObject;
        }

        public ActivityInvocationLog ActivityInvocationLog => _log;

        public override void BookHolidayRequestActivity(HolidayRequestEntity HolidayRequest)
        {
            _realObject.BookHolidayRequestActivity(HolidayRequest);
            _log.Log(new ActivityInvocation(nameof(BookHolidayRequestActivity), 
                new Dictionary<string, object>() { { nameof(HolidayRequest), HolidayRequest } }, typeof(void)));
        }

        public override bool CreateHolidayApprovalTaskActivity(HolidayRequestEntity HolidayRequest, string ResponsibleUser)
        {
            var returnValue =  _realObject.CreateHolidayApprovalTaskActivity(HolidayRequest, ResponsibleUser);
            _log.Log(new ActivityInvocation(nameof(CreateHolidayApprovalTaskActivity),
                new Dictionary<string, object>() { { nameof(HolidayRequest), HolidayRequest }, { nameof(ResponsibleUser), ResponsibleUser } }, returnValue));
            return returnValue;
        }

        public override void SendEmailActivity(string Body, string Subject, string ToMailAddress)
        {
            _realObject.SendEmailActivity(Body, Subject, ToMailAddress);
            _log.Log(new ActivityInvocation(nameof(SendEmailActivity),
                new Dictionary<string, object>() {
                    { nameof(Body), Body },
                    { nameof(Subject), Subject },
                    { nameof(ToMailAddress), ToMailAddress},
                }, typeof(void)));
        }

        public override void UpdateHolidayRequestStateActivity(HolidayRequestEntity HolidayRequest, EHolidayRequestState NewState)
        {
            _realObject.UpdateHolidayRequestStateActivity(HolidayRequest, NewState);
            _log.Log(new ActivityInvocation(nameof(UpdateHolidayRequestStateActivity),
                new Dictionary<string, object>() {
                    { nameof(HolidayRequest), HolidayRequest },
                    { nameof(NewState), NewState },
                }, typeof(void)));
        }
    }
}
