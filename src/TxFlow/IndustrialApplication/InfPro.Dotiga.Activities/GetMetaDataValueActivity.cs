using InfPro.Dotiga.ValueObjects;
using System;
using System.Activities;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InfPro.Dotiga.Activities
{
    public class GetMetaDataValueActivity<T> : NativeActivity<T>
    {
        public InArgument<DepotInstanceVO> DepotInstance { get; set; }
        public InArgument<string> MetaDataFieldName { get; set; }

        protected override void Execute(NativeActivityContext context)
        {

        }
    }
}
