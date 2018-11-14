using System;
using System.Activities;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HolidayApproval.Activities
{
    public class SendEmailActivity : NativeActivity
    {
        public InArgument<string> Subject { get; set; }
        public InArgument<string> Body { get; set; }

        public InArgument<string> ToMailAddress { get; set; }

        protected override void Execute(NativeActivityContext context)
        {

        }
    }
}
