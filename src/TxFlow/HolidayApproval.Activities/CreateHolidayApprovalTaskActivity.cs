using HolidayApproval.Entities;
using System;
using System.Activities;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HolidayApproval.Activities
{
    public class CreateHolidayApprovalTaskActivity : NativeActivity<bool>
    {
        public InArgument<HolidayRequestEntity> HolidayRequest { get; set; }

        public InArgument<string> ResponsibleUser { get; set; }

        protected override void Execute(NativeActivityContext context)
        {
            this.Result.Set(context, false);
        }
    }
}
