using System.IO;
using System.Runtime.InteropServices;
using System.Reflection.Emit;
using Microsoft.Pex.Framework.Using;
using System.Text;
using System;
// <copyright file="PexAssemblyInfo.cs">Copyright ©  2017</copyright>
using Microsoft.Pex.Framework.Coverage;
using Microsoft.Pex.Framework.Creatable;
using Microsoft.Pex.Framework.Instrumentation;
using Microsoft.Pex.Framework.Settings;
using Microsoft.Pex.Framework.Validation;

// Microsoft.Pex.Framework.Settings
[assembly: PexAssemblySettings(TestFramework = "VisualStudioUnitTest")]

// Microsoft.Pex.Framework.Instrumentation
[assembly: PexAssemblyUnderTest("TxFlow.TestWorkflow")]
[assembly: PexInstrumentAssembly("TxFlow.CSharpDSL")]

// Microsoft.Pex.Framework.Creatable
[assembly: PexCreatableFactoryForDelegates]

// Microsoft.Pex.Framework.Validation
[assembly: PexAllowedContractRequiresFailureAtTypeUnderTestSurface]
[assembly: PexAllowedXmlDocumentedException]

// Microsoft.Pex.Framework.Coverage
[assembly: PexCoverageFilterAssembly(PexCoverageDomain.UserOrTestCode, "TxFlow.CSharpDSL")]
[assembly: PexInstrumentType(typeof(Environment))]
[assembly: PexInstrumentType(typeof(DecoderReplacementFallback))]
[assembly: PexInstrumentAssembly("TestInterception")]
[assembly: PexUseType(typeof(ConstructorBuilder))]
[assembly: PexInstrumentType("mscorlib", "Microsoft.Win32.Win32Native")]
[assembly: PexInstrumentType(typeof(Marshal))]
[assembly: PexInstrumentType(typeof(AppDomain))]
[assembly: PexInstrumentType(typeof(Directory))]

