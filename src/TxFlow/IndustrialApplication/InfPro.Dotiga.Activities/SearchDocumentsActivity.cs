using InfPro.Dotiga.ValueObjects;
using System;
using System.Activities;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InfPro.Dotiga.Activities
{
    public class SearchDocumentsActivity : NativeActivity<IEnumerable<DocumentVO>>
    {
        public InArgument<DocumentSearchConfigVO> SearchConfig { get; set; }

        protected override void Execute(NativeActivityContext context)
        {

        }
    }
}
