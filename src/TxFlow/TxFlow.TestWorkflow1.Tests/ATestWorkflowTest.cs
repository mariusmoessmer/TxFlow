// <copyright file="ATestWorkflowTest.cs">Copyright ©  2017</copyright>

using System;
using Microsoft.Pex.Framework;
using Microsoft.Pex.Framework.Validation;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TxFlow.TestWorkflow;

namespace TxFlow.TestWorkflow.Tests
{
    [TestClass]
    [PexClass(typeof(ATestWorkflow))]
    [PexAllowedExceptionFromTypeUnderTest(typeof(ArgumentException), AcceptExceptionSubtypes = true)]
    [PexAllowedExceptionFromTypeUnderTest(typeof(InvalidOperationException))]
    public partial class ATestWorkflowTest
    {

        [PexMethod]
        public void Execute([PexAssumeUnderTest]ATestWorkflow target, int value)
        {
            target.Execute(value);
            // TODO: add assertions to method ATestWorkflowTest.Execute(ATestWorkflow, Int32)
        }
    }
}
