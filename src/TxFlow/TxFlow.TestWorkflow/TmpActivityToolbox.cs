using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TxFlow.TestWorkflow
{
    public interface TmpActivityToolbox
    {
        bool ExistsInCollection(List<int> test, int tmp);
        void WriteLine(string v);
    }
}
