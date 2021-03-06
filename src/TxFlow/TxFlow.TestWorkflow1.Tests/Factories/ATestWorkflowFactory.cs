using TxFlow.CSharpDSL;
using TxFlow.TestWorkflow;
// <copyright file="ATestWorkflowFactory.cs">Copyright ©  2017</copyright>

using System;
using Microsoft.Pex.Framework;

namespace TxFlow.TestWorkflow
{
    /// <summary>A factory for TxFlow.TestWorkflow.ATestWorkflow instances</summary>
    public static partial class ATestWorkflowFactory
    {
        /// <summary>A factory for TxFlow.TestWorkflow.ATestWorkflow instances</summary>
        [PexFactoryMethod(typeof(ATestWorkflow))]
        public static ATestWorkflow Create(AbstractActivityService activityService_abstractActivityService)
        {
            ATestWorkflow aTestWorkflow = new ATestWorkflow();
            ((AbstractWorkflow<AbstractActivityService>)aTestWorkflow)
              .RegisterActivityService(activityService_abstractActivityService);
            return aTestWorkflow;

            // TODO: Edit factory method of ATestWorkflow
            // This method should be able to configure the object in all possible ways.
            // Add as many parameters as needed,
            // and assign their values to each field by using the API.
        }
    }
}
