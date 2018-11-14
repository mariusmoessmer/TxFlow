using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.FindSymbols;
using System;
using System.Activities;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using TxFlow.CSharpDSL;
using TxFlow.WFBuilder;

namespace TxFlow.CSharpDSL.Transpiler
{
    public class CSharpToFlowChartTranspiler
    {
        private readonly Document _document;
        private readonly SemanticModel _semanticModel;
        private readonly SyntaxNode _syntaxRoot;
        private readonly WorkflowInfoCollector _collector;

        public CSharpToFlowChartTranspiler(Document document, SyntaxNode syntaxNode, SemanticModel model, WorkflowInfoCollector collector)
        {
            _document = document;
            _semanticModel = model;
            _syntaxRoot = syntaxNode;
            _collector = collector;
        }

        public Document Document
        {
            get
            {
                return this._document;
            }
        }

        internal WorkflowBuilder Compile()
        {
            WorkflowBuilder wb = new WorkflowBuilder(_collector.ClassDeclaration.Identifier.ToString(), _collector.Usings);

            wb.WithInArguments(_collector.ExecuteMethodDeclaration.ParameterList.Parameters.Select(executeParam =>
            {
                string paramName = executeParam.Identifier.ValueText;
                ITypeSymbol typeInfo = _semanticModel.GetTypeInfo(executeParam.Type).Type;
                string fullTypeName = CSharpSyntaxProcessor.getFullTypeName(typeInfo);

                return Tuple.Create(paramName, fullTypeName);
            }).ToArray());


            var blockSyntax = _collector.ExecuteMethodDeclaration.Body;


            new CSharpSyntaxProcessor(_document, _semanticModel, _syntaxRoot, _collector.ClassDeclaration).Process(wb.WorkflowPath, blockSyntax);

            wb.Validate();
            return wb;
        }

       

        
    }
}
