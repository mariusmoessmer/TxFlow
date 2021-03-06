using System;
// <copyright file="PexAssemblyInfo.cs">Copyright ©  2018</copyright>
using Microsoft.Pex.Framework.Coverage;
using Microsoft.Pex.Framework.Creatable;
using Microsoft.Pex.Framework.Instrumentation;
using Microsoft.Pex.Framework.Settings;
using Microsoft.Pex.Framework.Validation;

// Microsoft.Pex.Framework.Settings
[assembly: PexAssemblySettings(TestFramework = "VisualStudioUnitTest")]

// Microsoft.Pex.Framework.Instrumentation
[assembly: PexAssemblyUnderTest("InfPro.Dotiga.TxFlow")]
[assembly: PexInstrumentAssembly("Microsoft.VisualStudio.QualityTools.UnitTestFramework")]

// Microsoft.Pex.Framework.Creatable
[assembly: PexCreatableFactoryForDelegates]

// Microsoft.Pex.Framework.Validation
[assembly: PexAllowedContractRequiresFailureAtTypeUnderTestSurface]
[assembly: PexAllowedXmlDocumentedException]

// Microsoft.Pex.Framework.Coverage
[assembly: PexCoverageFilterAssembly(PexCoverageDomain.UserOrTestCode, "Microsoft.VisualStudio.QualityTools.UnitTestFramework")]
[assembly: PexInstrumentAssembly("InfPro.Dotiga.TxFlow")]
[assembly: PexInstrumentAssembly("TxFlow.CSharpDSL")]
[assembly: PexInstrumentAssembly("InfPro.Dotiga.ValueObjects")]
[assembly: PexInstrumentType(typeof(TimeZoneInfo))]
[assembly: PexInstrumentType("mscorlib", "System.Globalization.DaylightTimeStruct")]
[assembly: PexInstrumentType(typeof(Environment))]

