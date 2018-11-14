using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TxFlow.Debug.ValueObjects
{
    public class ActivitySourceLocationVO
    {
        #region fields

        public string ActivityId { get; set; }
        public int Line { get; set ; }
        public int Column { get; set; }

        public string ParentFlowChartName { get; set; }


        #endregion
    }
}
