using InfPro.Dotiga.ValueObjects;
using System;
using System.Activities;

namespace InfPro.Dotiga.Activities
{
    public class GetDepotInstanceActivity : NativeActivity<DepotInstanceVO>
    {
        public InArgument<string> DepotTypeName { get; set; }

        public InArgument<Guid> DepotInstanceID { get; set; }

        protected override void Execute(NativeActivityContext context)
        {
            throw new System.NotImplementedException();
        }
    }
}
