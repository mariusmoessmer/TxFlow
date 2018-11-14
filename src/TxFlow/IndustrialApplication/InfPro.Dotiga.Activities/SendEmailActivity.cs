using System.Activities;

namespace InfPro.Dotiga.Activities
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
