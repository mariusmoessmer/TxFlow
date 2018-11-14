using System;
using System.Activities;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InfPro.Dotiga.Activities
{
    public class SleepUntilActivity : NativeActivity
    {
        public InArgument<DateTime> Until { get; set; }

        protected override void Execute(NativeActivityContext context)
        {
        }
    }
}
