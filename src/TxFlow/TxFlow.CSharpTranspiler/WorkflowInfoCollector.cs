using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Collections.Generic;
using System.Linq;

namespace TxFlow.CSharpDSL.Transpiler
{
    public class WorkflowInfoCollector : CSharpSyntaxWalker
    {
        private readonly SemanticModel _semanticModel;


        public WorkflowInfoCollector(SemanticModel semanticModel)
        {
            this._semanticModel = semanticModel;
        }

        public ClassDeclarationSyntax ClassDeclaration { get; private set; } = null;
        public MethodDeclarationSyntax ExecuteMethodDeclaration { get; private set; } = null;
        public List<Tuple<string, string>> Usings { get; private set; } = new List<Tuple<string, string>>();

        public override void VisitClassDeclaration(ClassDeclarationSyntax node)
        {


            //if (node.AttributeLists.Any(x => x.Attributes.Any(attribute => attribute.GetText().ToString().Contains("TxFlowWorkflow"))))
            //{
            //    _classDeclaration = node;
            //}

            if(node.BaseList != null && node.BaseList.Types.Any(x =>
            {
                var sem = this._semanticModel.GetTypeInfo(x.Type);
                return sem.Type.OriginalDefinition.ToDisplayString().StartsWith("TxFlow.CSharpDSL.AbstractWorkflow<");
            }))
            {
                ClassDeclaration = node;
            }


            base.VisitClassDeclaration(node);
        }

        public override void VisitUsingDirective(UsingDirectiveSyntax node)
        {
            string fullName = node.Name.ToFullString();
            var symbol = _semanticModel.GetSymbolInfo(node.Name);

            Usings.Add(Tuple.Create(symbol.Symbol.ContainingAssembly?.Name, fullName));

            base.VisitUsingDirective(node);
        }

        public override void VisitMethodDeclaration(MethodDeclarationSyntax node)
        {
            if (node.Identifier.ValueText == "Execute")
            {
                ExecuteMethodDeclaration = node;
            }

            base.VisitMethodDeclaration(node);

        }
    }
}
