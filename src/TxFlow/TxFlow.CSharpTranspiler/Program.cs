using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Symbols;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;
using Microsoft.CodeAnalysis.MSBuild;
using TxFlow.WFBuilder;
using System.IO;
using System.Diagnostics;
using TxFlow.WFActivities;

namespace TxFlow.CSharpDSL.Transpiler
{
    class Program
    {
        static void Main(string[] args)
        {
            if (Debugger.IsAttached)
            {
                args = new[] { @"D:\InfPro.Dotiga.TxFlow\InfPro.Dotiga.TxFlow.csproj" };
            } else
            {
#if DEBUG
                Debugger.Launch();
#endif
            }

            if (args.Length < 1)
            {
                throw new Exception("Projectfile must be defined as first argument!");
            }
            string projectFilePath = args[0];
            if (!File.Exists(projectFilePath))
            {
                throw new Exception("Projectfile does not exist on '" + projectFilePath + "'");
            }    
            

            var workspace = MSBuildWorkspace.Create(new Dictionary<string, string> { { "CheckForSystemRuntimeDependency", "true" } });

            Console.WriteLine($"Opening .csproj-file from '{projectFilePath}'");
            var project = workspace.OpenProjectAsync(projectFilePath).Result
                .AddMetadataReference(MetadataReference.CreateFromFile(typeof(int).Assembly.Location));
            Console.WriteLine(".csproj-file loaded sucessfully");




            foreach(var mdRef in project.MetadataReferences)
            {
                System.Reflection.Assembly.LoadFrom(mdRef.Display);
            }

            var transpilerPerDocument = project.Documents.Select(document =>
            {
                SourceText text = document.GetTextAsync().Result;
                bool fastCheckOK = text.ToString().Contains(new string(typeof(CSharpDSL.AbstractWorkflow<>).Name.TakeWhile(c => c != '`').ToArray()));
                if (fastCheckOK)
                {
                    var semanticModel = document.GetSemanticModelAsync().Result;
                    var syntaxRoot = document.GetSyntaxRootAsync().Result;

                    var collector = new WorkflowInfoCollector(semanticModel);
                    collector.Visit(syntaxRoot);
                    if (collector.ClassDeclaration != null)
                    {
                        return new CSharpToFlowChartTranspiler(document, syntaxRoot, semanticModel, collector);
                    }
                }

                return null;
            }).Where(x=> x != null);

            foreach(var transpiler in transpilerPerDocument)
            {
                var wb = transpiler.Compile();
                string destination = Path.GetDirectoryName(projectFilePath) + "\\bin\\" + Path.GetFileNameWithoutExtension(transpiler.Document.FilePath) + ".xaml";
                wb.SerializeTo(destination);
                Console.WriteLine($"Successfully written: {destination}");
            }

            //wb.TestInvoke(new Dictionary<string, object>()
            //{
            //    { "HolidayRequest", new HolidayRequestEntity()},
            //});
        }
    }
}
