using Microsoft.VisualStudio.TestTools.UnitTesting;
using TxFlow.TestWorkflow;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TxFlow.CSharpDSL;
using System.IO;

namespace TxFlow.TestWorkflow.ManualTests
{
    [TestClass]
    public class ManualATestWorkflowTest
    {
        [DataRow(0, 0)]
        [DataRow(1, 1)]
        [DataRow(2, 1)]
        [DataRow(60, 1820529360)]
        [DataTestMethod]
        public void ManualTestExecute(int number, int result)
        {
            var mockedActivityToolbox = new MockedActivityToolbox();
            ATestWorkflow aTest = new ATestWorkflow();
            aTest.RegisterActivityToolbox(mockedActivityToolbox);

            aTest.Execute(number);
            Assert.IsTrue(mockedActivityToolbox.WriteLineTexts[0] == number.ToString());
            Assert.IsTrue(mockedActivityToolbox.WriteLineTexts[1] == result.ToString());
            if(result > 2)
            {
                Assert.IsTrue(mockedActivityToolbox.WriteLineTexts[2] == "Result higher than 2!");
            }
            

        }

        public class MockedActivityToolbox : AbstractActivityToolbox
        {
            public List<string> WriteLineTexts = new List<string>();

            public override void WriteLine(string Text, TextWriter TextWriter = null)
            {
                WriteLineTexts.Add(Text);
            }
        }
    }
}