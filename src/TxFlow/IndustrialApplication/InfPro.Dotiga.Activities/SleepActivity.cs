using System;
using System.Activities;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InfPro.Dotiga.Activities
{
    public class SleepActivity : NativeActivity
    {
        public InArgument<TimeSpan> TimeSpan { get; set; }

        protected override void Execute(NativeActivityContext context)
        {
        }
    }
}
