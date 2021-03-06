// <copyright file="ATestWorkflowTest.cs">Copyright ©  2017</copyright>
using System;
using Microsoft.Pex.Framework;
using Microsoft.Pex.Framework.Validation;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TxFlow.TestWorkflow;

namespace TxFlow.TestWorkflow.IntelliTests
{
    /// <summary>This class contains parameterized unit tests for ATestWorkflow</summary>
    [PexClass(typeof(ATestWorkflow))]
    [PexAllowedExceptionFromTypeUnderTest(typeof(InvalidOperationException))]
    [PexAllowedExceptionFromTypeUnderTest(typeof(ArgumentException), AcceptExceptionSubtypes = true)]
    [TestClassAttribute]
    public partial class ATestWorkflowTest
    {
        /// <summary>Test stub for Execute(Int32)</summary>
        [PexMethod]
        public void ExecuteTest([PexAssumeUnderTest]ATestWorkflow target, int value)
        {
            PexAssume.IsTrue(value < 500);
            target.Execute(value);
            // TODO: add assertions to method ATestWorkflowTest.ExecuteTest(ATestWorkflow, Int32)
        }
    }
}
