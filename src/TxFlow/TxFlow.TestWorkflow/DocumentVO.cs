using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TxFlow.TestWorkflow
{
    public class DocumentVO
    {
        private readonly Guid _documentID;
        private readonly string _name;

        public DocumentVO(Guid documentID, string name)
        {
            _documentID = documentID;
            _name = name;
        }

        public string Name => _name;

        public Guid DocumentID => _documentID;
    }
}
