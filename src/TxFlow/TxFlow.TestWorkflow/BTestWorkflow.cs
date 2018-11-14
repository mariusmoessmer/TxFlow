using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TxFlow.CSharpDSL;

namespace TxFlow.TestWorkflow
{


    public class BTestWorkflow
    {
        public TmpActivityToolbox Activities { get; set; }

        public void Execute(int tmp)
        {
            List<int> test = new List<int>() { 100, 200};

            bool result = Activities.ExistsInCollection(test, tmp);

            if(result)
            {
                Activities.WriteLine("Exists!");
            }else
            {
                Activities.WriteLine("Does not exist!");
            }

            Activities.WriteLine("Done");
        }
    }
}
