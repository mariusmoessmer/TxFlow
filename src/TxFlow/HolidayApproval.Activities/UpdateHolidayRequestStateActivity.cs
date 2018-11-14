using HolidayApproval.Entities;
using System;
using System.Activities;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HolidayApproval.Activities
{
    public class UpdateHolidayRequestStateActivity : NativeActivity
    {
        public InArgument<HolidayRequestEntity> HolidayRequest { get; set; }
        public InArgument<EHolidayRequestState> NewState { get; set; }

        protected override void Execute(NativeActivityContext context)
        {
            this.HolidayRequest.Get(context).State = this.NewState.Get(context);
        }
    }
}
