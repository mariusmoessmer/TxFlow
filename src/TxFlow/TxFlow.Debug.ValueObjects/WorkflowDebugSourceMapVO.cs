using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace TxFlow.Debug.ValueObjects
{
    public class WorkflowDebugSourceMapVO
    {
        #region fields

        private string _workflowName = null;
        private ActivitySourceLocationVO[] _activitySourceLocations = null;

        #endregion

        #region ctor

        public WorkflowDebugSourceMapVO()
        {

        }

        #endregion

        #region properties
        public string WorkflowName { get => _workflowName; set => _workflowName = value; }
        public string WorkflowDefinitionID { get; set; }

        public ActivitySourceLocationVO[] ActivitySourceLocations { get => _activitySourceLocations; set => _activitySourceLocations = value; }
        

        #endregion

        public void SerializeTo(string filePath)
        {
            using (Stream s = File.Open(filePath, FileMode.Create))
                _xmlSerializer.Value.Serialize(s, this);
        }

        private static Lazy<XmlSerializer> _xmlSerializer = new Lazy<XmlSerializer>(() =>
        {
            return new XmlSerializer(typeof(WorkflowDebugSourceMapVO));
        });


        public static WorkflowDebugSourceMapVO Deserialize(string filePath)
        {
            return (WorkflowDebugSourceMapVO)(_xmlSerializer.Value.Deserialize(File.OpenRead(filePath)));
        }

        public void AddActivitySourceLocation(string id, int line, int character)
        {
            throw new Exception("Only for masterthesis");
        }
    }
}
