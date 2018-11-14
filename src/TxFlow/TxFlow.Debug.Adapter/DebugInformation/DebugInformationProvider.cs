using Microsoft.VisualStudio.Shared.VSCodeDebugProtocol.Messages;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TxFlow.Debug.ValueObjects;

namespace TxFlow.Debug.Adapter.DebugInformation
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


        private Dictionary<string, WorkflowDebugInformation> _debugInformationsPerWorkflowDefinitionID = new Dictionary<string, WorkflowDebugInformation>(StringComparer.OrdinalIgnoreCase);


        internal List<Breakpoint> SetBreakPoints(string path, List<SourceBreakpoint> sourceBreakpoints)
        {
            string pathToTxDBFile = Path.GetFullPath(Path.Combine(Path.GetDirectoryName(path), "bin", Path.GetFileNameWithoutExtension(path) + ".txdb"));
            if(File.Exists(pathToTxDBFile))
            {
                var debugInfo = WorkflowDebugSourceMapVO.Deserialize(pathToTxDBFile);
                if(debugInfo != null)
                {
                    if (!_debugInformationsPerWorkflowDefinitionID.ContainsKey(debugInfo.WorkflowDefinitionID))
                    {
                        _debugInformationsPerWorkflowDefinitionID[debugInfo.WorkflowDefinitionID] = new WorkflowDebugInformation(path, debugInfo);
                    }

                    var result = _debugInformationsPerWorkflowDefinitionID[debugInfo.WorkflowDefinitionID].SetBreakPoints(path, sourceBreakpoints);
                    if (result != null)
                    {
                        // match
                        return result.Select(x => new Breakpoint(true, line: x)).ToList();
                    }
                }
            }
            // nothing found
            return sourceBreakpoints.Select(x=> new Breakpoint(false, message: $"File {path} not registered!")).ToList();
        }


        //private List<WorkflowDebugInformation> _debugInformations = new List<WorkflowDebugInformation>();

        internal WorkflowDebugInformation TryGetDebugInformation(string workflowDefinitionID)
        {
            WorkflowDebugInformation result;
            if(_debugInformationsPerWorkflowDefinitionID.TryGetValue(workflowDefinitionID, out result))
            {
                return result;
            }

            return null;
        }

        //internal void InitializeProjectDir(string projectDir)
        //{
        //    foreach(var txdbFile in Directory.GetFiles(projectDir + "\\bin", "*.txdb"))
        //    {
        //        var wfDebug = new WorkflowDebugInformation(CSharpWorkflowDebugVO.Deserialize(txdbFile)) ;

        //        lock (_debugInformations)
        //        {
        //            _debugInformations.Add(wfDebug);
        //        }
        //    }
        //}
    }
}
