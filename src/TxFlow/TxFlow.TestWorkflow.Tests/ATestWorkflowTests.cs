using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TxFlow.CSharpDSL;

namespace TxFlow.TestWorkflow.Tests
{
    [TestClass]
    public class ATestWorkflowTests
    {
        [DataTestMethod]
        [DataRow(0, 0)]
        [DataRow(1, 1)]
        [DataRow(2, 1)]
        [DataRow(3, 2)]
        [DataRow(4, 3)]
        [DataRow(5, 5)]
        [DataRow(6, 8)]
        public void TestCase1(int value, int expectedResult)
        {

            ATestWorkflow testWorkflow = new ATestWorkflow();

            // register activityservice for this testcase to mock activity-calls
            TestCase1ActivityService activityService = new TestCase1ActivityService();
            testWorkflow.RegisterActivityService(activityService);


            // execute workflow
            //testWorkflow.Execute(value);

            // check if writelines are called correctly:
            // 1. initial-value should be printed to console
            Assert.AreEqual(value.ToString(), activityService.WriteLineTexts[0]);
            // 2. result should be printed to console
            Assert.AreEqual(expectedResult.ToString(), activityService.WriteLineTexts[1]);

            // if result is higher than 2 --> "Result higher than 2!" should be printed to console
            if (expectedResult > 2)
            {
                Assert.AreEqual("Result higher than 2!", activityService.WriteLineTexts[2]);
            }

        }

        private class TestCase1ActivityService : AbstractActivityService
        {
            private readonly List<string> writeLineTexts = new List<string>();

            public List<string> WriteLineTexts { get => writeLineTexts; }

            public override void WriteLine(string Text, TextWriter TextWriter = null)
            {
                this.writeLineTexts.Add(Text);
            }
        }

        
    }
}
