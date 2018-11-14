using System;
using System.Activities;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InfPro.Dotiga.Activities
{
    public class CreateTaskActivity : NativeActivity<string>
    {
        public InArgument<string> ResponsibleUser { get; set; }

        public InArgument<Dictionary<string, object>> ReadonlyTaskFields { get; set; }

        public InArgument<Dictionary<string, object>> EditableTaskFields { get; set; }

        public InArgument<List<string>> TaskCommands { get; set; }

        protected override void Execute(NativeActivityContext context)
        {

        }
    }
}
