// <copyright file="RequestProofOfDeliveryWFTestsTest.cs">Copyright ©  2018</copyright>

using System;
using System.IO;
using InfPro.Dotiga.ValueObjects;
using Microsoft.Pex.Framework;
using Microsoft.Pex.Framework.Validation;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace InfPro.Dotiga.TxFlow.PexTests
{
    [TestClass]
    [PexClass]
    [PexAllowedExceptionFromTypeUnderTest(typeof(ArgumentException), AcceptExceptionSubtypes = true)]
    [PexAllowedExceptionFromTypeUnderTest(typeof(InvalidOperationException))]
    public partial class RequestProofOfDeliveryWFTestsTest
    {
        private static string logFileNamePrefix = null;
        private static int executionNum = 0;



        private class PexInfProDotigaActivityToolboxEx : PexInfProDotigaActivityToolbox
        {
            public override T GetMetaDataValueActivity<T>(DepotInstanceVO DepotInstance, string MetaDataFieldName)
            {
                var value = base.GetMetaDataValueActivity<T>(DepotInstance, MetaDataFieldName);
                if (value is DateTime)
                {
                    PexAssume.IsTrue(((DateTime)(object)value).Year == 2018);
                }

                return value;
            }
        }


        //[PexMethod(MaxConstraintSolverTime = 40,
        //    MaxConstraintSolverMemory = int.MaxValue,
        //    MaxBranches = int.MaxValue,
        //    MaxCalls = int.MaxValue,
        //    MaxStack = int.MaxValue,
        //    MaxConditions = int.MaxValue,
        //    MaxRuns = int.MaxValue,
        //    MaxRunsWithoutNewTests = int.MaxValue,
        //    MaxRunsWithUniquePaths = int.MaxValue,
        //    MaxExceptions = int.MaxValue,
        //    TestEmissionBranchHits = int.MaxValue
        //    )]
        [PexMethod(MaxConstraintSolverTime = 10, MaxConstraintSolverMemory = int.MaxValue, Timeout = 240)]
        public void ExploreRequestProofOfDeliveryWF(DepotInstanceVO depotInstance)
        {
            PexAssume.IsNotNull(depotInstance);

            var activityToolboxLogger = new InfProDotigaActivityToolboxLogger(
                new PexInfProDotigaActivityToolboxEx());
            var workflow = new RequestProofOfDeliveryWF();
            workflow.RegisterActivityToolbox(activityToolboxLogger);
            Exception exception = null;
            try
            {
                workflow.Execute(depotInstance);
            }catch(Exception ex)
            {
                exception = ex;
            }finally
            {
                PexSymbolicValue.IgnoreComputation(() =>
                {
                    if(RequestProofOfDeliveryWFTestsTest.logFileNamePrefix == null)
                    {

                        logFileNamePrefix = System.IO.Path.GetTempPath() + $"ActivityLog_{DateTime.Now.ToString("ddMMyyyyHHmmss")}\\";
                        Directory.CreateDirectory(logFileNamePrefix);

                    }
                    string logFileName = logFileNamePrefix + (executionNum++)+ ".json";
                    activityToolboxLogger.ActivityInvocationLog.WriteAsJson(logFileName);

                    if (exception != null)
                    {
                        System.IO.File.AppendAllText(logFileName, exception.ToString());
                    }

                    PexObserve.ValueForViewing("ActivityInvocations", logFileName);
                });
            }
        }

    }
}
