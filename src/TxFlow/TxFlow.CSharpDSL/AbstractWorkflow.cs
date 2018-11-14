using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TxFlow.CSharpDSL
{
    public abstract class AbstractWorkflow<TActivityToolbox>
    {
        private TActivityToolbox _activityToolbox = default(TActivityToolbox);

        public AbstractWorkflow()
        {

        }

        public TActivityToolbox Activities { get { return _activityToolbox; } }

        public void RegisterActivityToolbox(TActivityToolbox activityToolbox)
        {
            this._activityToolbox = activityToolbox;
        }
    }
}
