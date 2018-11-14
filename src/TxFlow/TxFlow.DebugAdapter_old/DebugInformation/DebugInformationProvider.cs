using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TxFlow.Debug.ValueObjects;
using TxFlow.DebugAdapter.Protocol;

namespace TxFlow.DebugAdapter.DebugInformation
{
    class DebugInformationProvider
    {
        #region singleton

        private static Lazy<DebugInformationProvider> _instance = new Lazy<DebugInformationProvider>(() => new DebugInformationProvider(), true);

        private DebugInformationProvider()
        {
        }

        public static DebugInformationProvider Instance
        {
            get
            {
                return _instance.Value;
            }

        }

        #endregion

        internal IEnumerable<Breakpoint> SetBreakPoints(string path, int[] clientLines)
        {
            HashSet<Breakpoint> verified = new HashSet<Breakpoint>();
            foreach(var debugInformation in _debugInformations)
            {
                var verifiedTmp = debugInformation.SetBreakPoints(path, clientLines);

                foreach (var l in verifiedTmp)
                {
                    verified.Add(new Breakpoint(true, l));
                }
            }

            return verified;
        }


        private List<WorkflowDebugInformation> _debugInformations = new List<WorkflowDebugInformation>();

        internal WorkflowDebugInformation TryGetDebugInformation(string workflowName)
        {
            return _debugInformations.FirstOrDefault(x => x.WorkflowName == workflowName);
        }

        internal void InitializeProjectDir(string projectDir)
        {
            foreach(var txdbFile in Directory.GetFiles(projectDir + "\\bin", "*.txdb"))
            {
                var wfDebug = new WorkflowDebugInformation(CSharpWorkflowDebugVO.Deserialize(txdbFile)) ;

                lock (_debugInformations)
                {
                    _debugInformations.Add(wfDebug);
                }
            }
        }
    }
}
