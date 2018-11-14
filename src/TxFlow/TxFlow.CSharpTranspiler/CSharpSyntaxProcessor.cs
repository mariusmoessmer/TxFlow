using Microsoft.CodeAnalysis;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using TxFlow.WFBuilder;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.FindSymbols;
using System.Activities;
using System.Collections.Immutable;
using TxFlow.CSharpDSL;
using Microsoft.CodeAnalysis.Editing;
using TxFlow.Debug.ValueObjects;
using TxFlow.WFActivities;

namespace TxFlow.CSharpDSL.Transpiler
{
    class CSharpSyntaxProcessor
    {
        private Document _document = null;
        private SemanticModel semanticModel;
        private SyntaxNode syntaxRoot;
        private ClassDeclarationSyntax classDeclaration;
        private readonly Tuple<string, string> returnParamAndName;

        public CSharpSyntaxProcessor(Document document, SemanticModel semanticModel, SyntaxNode syntaxRoot, ClassDeclarationSyntax classDeclaration, Tuple<string, string> returnParamAndName = null)
        {
            _document = document;
            this.semanticModel = semanticModel;
            this.syntaxRoot = syntaxRoot;
            this.classDeclaration = classDeclaration;
            this.returnParamAndName = returnParamAndName;
        }

        internal void Process(WorkflowPath workflowPath, StatementSyntax blockSyntax)
        {
            this.processStatement(workflowPath, blockSyntax);
        }

        public static string getFullTypeName(ITypeSymbol typeInfo)
        {
            if (typeInfo is INamedTypeSymbol)
            {
                return getFullTypeName(typeInfo as INamedTypeSymbol);
            }

            throw new Exception("Only named types are alllowed!");
        }

        public static string getFullTypeName(INamedTypeSymbol namedTypeInfo)
        {
            //return typeInfo.ContainingNamespace.ToString() + "." + typeInfo.Name;


            string result = namedTypeInfo.ToDisplayString(new SymbolDisplayFormat(SymbolDisplayGlobalNamespaceStyle.Omitted, SymbolDisplayTypeQualificationStyle.NameAndContainingTypesAndNamespaces, SymbolDisplayGenericsOptions.IncludeTypeParameters));
            if (namedTypeInfo.Arity > 0) // if its a generic type
            {
                result = result.Replace("<", "`" + namedTypeInfo.Arity + "[[").Replace(">", "]]");
            }



            return result;
            //return namedTypeInfo.ToString();
        }

        private void processBlock(WorkflowPath flowChart, BlockSyntax blockSyntax)
        {
            foreach (StatementSyntax statement in blockSyntax.Statements)
            {
                processStatement(flowChart, statement);
            }
        }

// just for MasterThesis.docx
//System.Activities.Statements.FlowNode processStatement(
//    StatementSyntax statement)
//{
//    if (statement.IsKind(SyntaxKind.IfStatement))
//    {
//        IfStatementSyntax ifStatement = (statement as IfStatementSyntax);
//        var flowDecision = new System.Activities.Statements.FlowDecision()
//        {
//            Condition = new Microsoft.CSharp.Activities.CSharpValue<bool>(
//                ifStatement.Condition.ToString()
//            ),
//            True = processStatement(ifStatement.Statement),
//            False = processStatement(ifStatement.Else?.Statement)
//        };

//        return flowDecision;
//    }
//    else if (statement.IsKind(SyntaxKind.ForStatement))
//    {
//        // ...
//    } // ...

//    return null;
//}
//System.Activities.Statements.FlowNode processStatement(
//    StatementSyntax statement, WorkflowDebugSourceMapVO debugSourceMap)
//{
//    if (statement.IsKind(SyntaxKind.IfStatement))
//    {
//        IfStatementSyntax ifStatement = (statement as IfStatementSyntax);
//        var flowDecision = new System.Activities.Statements.FlowDecision()
//        {
//            Condition = new Microsoft.CSharp.Activities.CSharpValue<bool>(
//                ifStatement.Condition.ToString()
//            ),
//            True = processStatement(ifStatement.Statement),
//            False = processStatement(ifStatement.Else?.Statement)
//        };

//        var csharpLocation = ifStatement.Condition.GetLocation()
//                                .GetLineSpan().StartLinePosition;
//        debugSourceMap.AddActivitySourceLocation(
//            flowDecision.Condition.Id, 
//            csharpLocation.Line, 
//            csharpLocation.Character
//        );
//        return flowDecision;
//    }
//    else if (statement.IsKind(SyntaxKind.ForStatement))
//    {
//        // ...
//    } // ...
//    return null;
//}

        private void processStatement(WorkflowPath flowChart, StatementSyntax statement)
        {
            if (statement.IsKind(SyntaxKind.ExpressionStatement))
            {
                processExpression(flowChart, (statement as ExpressionStatementSyntax).Expression);
            }
            else if (statement.IsKind(SyntaxKind.Block))
            {
                BlockSyntax blockSyntaxInner = statement as BlockSyntax;
                processBlock(flowChart, blockSyntaxInner);
            }
            else if (statement.IsKind(SyntaxKind.ForStatement))
            {
                ForStatementSyntax forStatement = statement as ForStatementSyntax;
                flowChart.FlowChart(null, "ForLoop As While", (wP) =>
                {
                    this.processVariableDeclaration(wP, forStatement.Declaration);

                    wP.While(forStatement.Condition.ToString(), (workflowPath) =>
                    {
                        processStatement(workflowPath, forStatement.Statement);
                        if (forStatement.Incrementors != null)
                        {
                            foreach (var inc in forStatement.Incrementors)
                            {
                                this.processExpression(workflowPath, inc);
                            }
                        }
                    });

                });
            }
            else if (statement.IsKind(SyntaxKind.ForEachStatement))
            {
                ForEachStatementSyntax forEachSyntax = statement as ForEachStatementSyntax;
                throw new NotImplementedException(forEachSyntax.ToFullString());
            }
            else if (statement.IsKind(SyntaxKind.IfStatement))
            {
                IfStatementSyntax ifStatement = (statement as IfStatementSyntax);
                processIf(flowChart, ifStatement);
            }
            else if (statement.IsKind(SyntaxKind.LocalDeclarationStatement))
            {
                LocalDeclarationStatementSyntax localDeclarationStatement = (statement as LocalDeclarationStatementSyntax);
                var declaration = localDeclarationStatement.Declaration;
                processVariableDeclaration(flowChart, declaration);
            }
            else if (statement.IsKind(SyntaxKind.ReturnStatement))
            {
                ReturnStatementSyntax returnStatement = (statement as ReturnStatementSyntax);
                if (this.returnParamAndName != null)
                {
                    assign(flowChart, returnParamAndName, returnStatement.Expression);
                }

                flowChart.Return();
            }
            else if (statement.IsKind(SyntaxKind.SwitchStatement))
            {
                SwitchStatementSyntax switchStatement = statement as SwitchStatementSyntax;

                Action<WorkflowPath> defaultworkflowPath = null;

                List<Tuple<Action<WorkflowPath>, List<object>>> cases = new List<Tuple<Action<WorkflowPath>, List<object>>>();

                foreach (var section in switchStatement.Sections)
                {
                    Action<WorkflowPath> wPAction = (workflowPath) =>
                    {
                        foreach (var st in section.Statements)
                        {
                            processStatement(workflowPath, st);
                        }
                    };

                    List<object> labels = new List<object>();

                    foreach (var tmp in section.Labels)
                    {
                        if (tmp.IsKind(SyntaxKind.CaseSwitchLabel))
                        {
                            CaseSwitchLabelSyntax caseSwitch = tmp as CaseSwitchLabelSyntax;
                            Optional<object> optional = semanticModel.GetConstantValue(caseSwitch.Value);
                            if (optional.HasValue)
                            {
                                labels.Add(optional.Value);
                            }
                            else
                            {
                                throw new NotImplementedException(caseSwitch.ToFullString());
                            }

                        }
                        else if (tmp.IsKind(SyntaxKind.DefaultSwitchLabel))
                        {
                            defaultworkflowPath = wPAction;
                        }
                        else
                        {
                            throw new NotImplementedException(tmp.ToFullString());
                        }
                    }

                    cases.Add(Tuple.Create(wPAction, labels));

                }



                flowChart.Switch(getFullTypeName(semanticModel.GetTypeInfo(switchStatement.Expression).ConvertedType),
                    switchStatement.Expression.ToString(),

                    defaultworkflowPath,
                    cases
                    );

            }
            else if (statement.IsKind(SyntaxKind.BreakStatement))
            {
                flowChart.Break();
            }

            else if (statement.IsKind(SyntaxKind.WhileStatement))
            {
                WhileStatementSyntax whileStatement = statement as WhileStatementSyntax;

                flowChart.While(whileStatement.Condition.ToString(), (workflowPath) =>
                {
                    processStatement(workflowPath, whileStatement.Statement);
                });

            }
            else if (statement.IsKind(SyntaxKind.DoStatement))
            {
                DoStatementSyntax doStatement = statement as DoStatementSyntax;
                flowChart.DoWhile(doStatement.Condition.ToString(), (workflowPath) =>
                {
                    processStatement(workflowPath, doStatement.Statement);
                });
            }
            else if (statement.IsKind(SyntaxKind.TryStatement))
            {
                TryStatementSyntax tryStatement = statement as TryStatementSyntax;

                Action<WorkflowPath> finallyPath = null;

                if (tryStatement.Finally != null)
                {
                    finallyPath = (fPath) =>
                    {
                        this.processBlock(fPath, tryStatement.Finally.Block);
                    };
                }

                flowChart.TryCatchFinally((tryWorkflowPath) =>
                {
                    processStatement(tryWorkflowPath, tryStatement.Block);
                }, tryStatement.Catches.Select(
                            x =>
                            {
                                string fullTypeName = getFullTypeName(semanticModel.GetTypeInfo(x.Declaration.Type).ConvertedType);
                                return new CatchBuilder(fullTypeName, x.Declaration.Identifier.ToString(), cp => this.processBlock(cp, x.Block));
                            }
                        )
                , finallyPath);
            }
            else if (statement.IsKind(SyntaxKind.ThrowStatement))
            {
                ThrowStatementSyntax throwStatement = statement as ThrowStatementSyntax;

                if (throwStatement.Expression != null)
                {
                    flowChart.Throw(WFActivityTypeHelper.CreateArgumentExpression(throwStatement.Expression.ToString(), typeof(System.Exception).FullName));
                }
                else
                {
                    flowChart.Rethrow();
                }
            }
            else
            {
                throw new NotImplementedException(statement.Kind().ToString());
            }
        }

        private void processVariableDeclaration(WorkflowPath flowChart, VariableDeclarationSyntax declaration)
        {
            string fullTypeName = getFullTypeName(semanticModel.GetTypeInfo(declaration.Type).ConvertedType);
            foreach (var variable in declaration.Variables)
            {
                flowChart.DeclareVariable(fullTypeName, variable.Identifier.ToString());
                if (variable.Initializer != null)
                {
                    var expression = variable.Initializer.Value;

                    Tuple<string, string> v = Tuple.Create(variable.Identifier.ToString(), fullTypeName);
                    assign(flowChart, v, expression);
                }
            }
        }

        private void assign(WorkflowPath flowChart, Tuple<string, string> variable, ExpressionSyntax expression)
        {
            //if (isActivityInvocation(expression))
            //{
            //    this.processExpression(flowChart, expression, variable);
            //}
            //else if (containsActivityCalls(expression))
            //{
            //    throw new NotImplementedException("nested activityinvocation is not implemented! Source: " + expression.ToFullString());
            //}
            //else
            //{
            //    if(expression.IsKind(SyntaxKind.InvocationExpression))
            //    {
            //        processInvocationExpression(flowChart, expression as InvocationExpressionSyntax, variable);
            //    }else
            //    {
            //        flowChart.Assign(variable.Item1, variable.Item2, expression.ToString(), variable.Item2);
            //    }
            //}

            if (expression.IsKind(SyntaxKind.InvocationExpression))
            {
                processInvocationExpression(flowChart, expression as InvocationExpressionSyntax, variable);
            }
            else
            {
                flowChart.Assign(createDebugLocation(flowChart, expression), variable.Item1, variable.Item2, expression.ToString(), variable.Item2);
            }
        }

        private bool isActivityInvocation(ExpressionSyntax node)
        {
            var symbol = semanticModel.GetSymbolInfo(node);
            if (symbol.Symbol == null)
            {
                return false;
            }

            //var tmp = symbol.Symbol.OriginalDefinition;
            var containingType = symbol.Symbol.OriginalDefinition.ContainingType;
            string activityName = symbol.Symbol.Name;

            return containingType.ToDisplayString().EndsWith("ActivityToolbox");
        }

        private class CheckForActivityCalls : CSharpSyntaxWalker
        {
            private CSharpSyntaxProcessor _processor;

            public bool HasActivityCalls = false;

            public CheckForActivityCalls(CSharpSyntaxProcessor processor)
            {
                this._processor = processor;
            }

            public override void VisitMemberAccessExpression(MemberAccessExpressionSyntax node)
            {
                if (this._processor.isActivityInvocation(node))
                {
                    HasActivityCalls = true;
                    return;
                }
                base.VisitMemberAccessExpression(node);


            }
        }

        private bool containsActivityCalls(ExpressionSyntax value)
        {
            var tmp = new CheckForActivityCalls(this);
            tmp.Visit(value);

            return tmp.HasActivityCalls;
        }

        private void processIf(WorkflowPath flowChart, IfStatementSyntax ifStatement)
        {

            Action<WorkflowPath> elseWorkflowPath = null;
            if (ifStatement.Else != null && ifStatement.Else.Statement != null)
            {
                elseWorkflowPath = (workflowPath) => processStatement(workflowPath, ifStatement.Else.Statement);
            }

            flowChart.FlowDecision(this.createDebugLocation(flowChart, ifStatement.Condition), ifStatement.Condition.ToString(), (workflowPath) =>
            {
                processStatement(workflowPath, ifStatement.Statement);
            }, elseWorkflowPath);
        }

        private void processExpression(WorkflowPath flowChart, ExpressionSyntax expression, Tuple<string, string> returnParamNameAndType = null)
        {

            if (expression.IsKind(SyntaxKind.InvocationExpression))
            {
                var invocationExpression = (expression as InvocationExpressionSyntax);
                processInvocationExpression(flowChart, invocationExpression, returnParamNameAndType);

            }
            else if (expression.IsKind(SyntaxKind.SimpleAssignmentExpression))
            {
                AssignmentExpressionSyntax assExp = expression as AssignmentExpressionSyntax;

                var leftTypeInfo = semanticModel.GetTypeInfo(assExp.Left);
                var rightTypeInfo = semanticModel.GetTypeInfo(assExp.Right);


                //flowChart.Assign(assExp.Left.ToString(), getFullTypeName(leftTypeInfo.ConvertedType), assExp.Right.ToString(), getFullTypeName(rightTypeInfo.ConvertedType));

                Tuple<string, string> v = Tuple.Create(assExp.Left.ToString(), getFullTypeName(leftTypeInfo.ConvertedType));
                assign(flowChart, v, assExp.Right);

            }
            else if (expression.IsKind(SyntaxKind.PostIncrementExpression))
            {
                PostfixUnaryExpressionSyntax p = expression as PostfixUnaryExpressionSyntax;
                //flowChart.Assign(p.Operand.)
                string varName = p.Operand.ToString();
                var type = semanticModel.GetTypeInfo(p.Operand);

                string typeName = getFullTypeName(type.ConvertedType);

                flowChart.Assign(createDebugLocation(flowChart, expression), varName, typeName, varName + " + 1", typeName);
            }
            //else if (expression.IsKind(SyntaxKind.PostDecrementExpression))
            //{
            //}
            //else if (expression.IsKind(SyntaxKind.PreIncrementExpression))
            //{

            //}
            //else if (expression.IsKind(SyntaxKind.PreDecrementExpression))
            //{

            //}
            else
            {
                throw new NotImplementedException(expression.ToFullString());
            }
        }

        private ActivitySourceLocationVO createDebugLocation(WorkflowPath wfPath, CSharpSyntaxNode node)
        {
            var linePosition = node.GetLocation().GetLineSpan().StartLinePosition;
            return new ActivitySourceLocationVO()
            {
                ParentFlowChartName = wfPath.FlowChartDisplayName,
                Column = linePosition.Character + 1,
                Line = linePosition.Line + 1,
            };
        }

        private void processInvocationExpression(WorkflowPath flowChart, InvocationExpressionSyntax invocationExpression, Tuple<string, string> returnParamNameAndType = null)
        {
            if (!invocationExpression.Expression.IsKind(SyntaxKind.IdentifierName)
                                && !invocationExpression.Expression.IsKind(SyntaxKind.SimpleMemberAccessExpression))
            {
                throw new NotImplementedException(invocationExpression.ToFullString());
            }

            var symbol = semanticModel.GetSymbolInfo(invocationExpression);
            
            string activityName = symbol.Symbol.Name;

            if (isActivityInvocation(invocationExpression))
            {
                var msymbol = (IMethodSymbol)symbol.Symbol;

                flowChart.ActivityInvocation(this.createDebugLocation(flowChart, invocationExpression), activityName, msymbol.TypeArguments.Select(x => getFullTypeName(x)).ToArray(), getParamValues(flowChart, invocationExpression), returnParamNameAndType);
            }
            else
            {
                var containingType = symbol.Symbol.OriginalDefinition.ContainingType;

                INamedTypeSymbol typeSymbolOfClass = semanticModel.GetDeclaredSymbol(classDeclaration);
                if (typeSymbolOfClass == containingType)
                {
                    var tmp = symbol.Symbol.OriginalDefinition.DeclaringSyntaxReferences;
                    var syntaxNode = tmp.First().GetSyntax();

                    if (syntaxNode.IsKind(SyntaxKind.MethodDeclaration))
                    {
                        var methodDeclarationSyntax = (MethodDeclarationSyntax)syntaxNode;

                        flowChart.FlowChart(createDebugLocation(flowChart, invocationExpression), activityName, (workflowPath) =>
                        {

                            var documentEditor = DocumentEditor.CreateAsync(this._document).Result;

                            documentEditor.TrackNode(this.classDeclaration);
                            documentEditor.TrackNode(methodDeclarationSyntax);

                            var argEnum = invocationExpression.ArgumentList.Arguments.GetEnumerator();
                            var paramEnum = methodDeclarationSyntax.ParameterList.Parameters.GetEnumerator();

                            while (paramEnum.MoveNext())
                            {
                                // TODO refactor me!!!!!!!!!!!!!!!
                                var executeParam = paramEnum.Current;

                                bool argExistsForParm = argEnum.MoveNext();
                                var arg = argExistsForParm ? argEnum.Current : null;
                                
                                if(executeParam.Modifiers.Any()) // ok its a ref-or-out-variable
                                {
                                    string argName = arg.Expression.ToString();
                                    if (argName != executeParam.Identifier.ToString())
                                    {
                                        documentEditor.ReplaceNode(executeParam, executeParam.WithIdentifier(SyntaxFactory.Identifier(argName)));


                                        // we have to replace variable name
                                        var paramSymbol = semanticModel.GetDeclaredSymbol(executeParam);

                                        var references = Microsoft.CodeAnalysis.FindSymbols.SymbolFinder.FindReferencesAsync(paramSymbol, _document.Project.Solution, new Document[] { _document }.ToImmutableHashSet()).Result;
                                        foreach (ReferencedSymbol reference in references)
                                        {
                                            foreach (var referenceLocation in reference.Locations)
                                            {
                                                var token = methodDeclarationSyntax.Body.FindNode(referenceLocation.Location.SourceSpan);
                                                if (!token.IsMissing)
                                                {
                                                    documentEditor.ReplaceNode(token, SyntaxFactory.IdentifierName(argName));
                                                }
                                            }
                                        }

                                    }
                                }else
                                {
                                    string paramName = executeParam.Identifier.ValueText;
                                    ITypeSymbol typeInfo = semanticModel.GetTypeInfo(executeParam.Type).Type;
                                    string fullTypeName = getFullTypeName(typeInfo);

                                    workflowPath.DeclareVariable(fullTypeName, paramName);


                                    
                                    ActivityWithResult val = null;
                                    if(argExistsForParm)
                                    {
                                        val = createActivityWithResultForArgument(flowChart, arg);
                                    }
                                    else if (executeParam.Default != null && executeParam.Default.Value != null)
                                    {
                                        Optional<object> optional = semanticModel.GetConstantValue(executeParam.Default.Value);
                                        val = WFActivityTypeHelper.CreateArgumentConstantValue(optional.Value, optional.Value.GetType().FullName);
                                    }
                                    else
                                    {
                                        throw new NotImplementedException(invocationExpression.ToFullString());
                                    }


                                    workflowPath.Assign(createDebugLocation(flowChart, methodDeclarationSyntax), paramName, fullTypeName, val);
                                }
                            }

                            var newDoc = documentEditor.GetChangedDocument();
                            var newSyntaxRoot = newDoc.GetSyntaxRootAsync().Result;
                            var newClassDeclaration = newSyntaxRoot.GetCurrentNode(this.classDeclaration);
                            new CSharpSyntaxProcessor(newDoc, newDoc.GetSemanticModelAsync().Result, newSyntaxRoot, newClassDeclaration, returnParamNameAndType)
                            .Process(workflowPath, newSyntaxRoot.GetCurrentNode(methodDeclarationSyntax).Body);
                        });
                    }
                    else
                    {
                        throw new NotImplementedException(invocationExpression.ToFullString());
                    }
                }
                else if(returnParamNameAndType != null)
                {
                    flowChart.Assign(createDebugLocation(flowChart, invocationExpression), returnParamNameAndType.Item1, returnParamNameAndType.Item2, invocationExpression.ToString(), returnParamNameAndType.Item2);
                }
                else
                {
                    // it must be a 
                    throw new NotImplementedException(invocationExpression.ToFullString());
                }
            }
        }

        private List<ActivityWithResult> getParamValues(WorkflowPath flowChart, InvocationExpressionSyntax invocationExpression)
        {
            var invocationArguments = invocationExpression.ArgumentList;

            List<ActivityWithResult> paramValues = new List<ActivityWithResult>();

            foreach (ArgumentSyntax invocationArg in invocationArguments.Arguments)
            {
                var res = createActivityWithResultForArgument(flowChart, invocationArg);
                if(res != null)
                    paramValues.Add(res);
            }

            return paramValues;
        }

        private ActivityWithResult createActivityWithResultForArgument(WorkflowPath flowChart, ArgumentSyntax invocationArg)
        {
            Optional<object> optional = semanticModel.GetConstantValue(invocationArg.Expression);


            var typeName = getFullTypeName(semanticModel.GetTypeInfo(invocationArg.Expression).ConvertedType);
            if (optional.HasValue)
            {
                return WFActivityTypeHelper.CreateArgumentConstantValue(optional.Value, typeName);
            }
            else
            {
                if (invocationArg.Expression is LambdaExpressionSyntax)
                {
                    // if an lambdaexpression is set es parameter-value --> create flowchart for it 
                    throw new NotImplementedException(invocationArg.Expression.ToFullString());
                }
                else
                {
                    var symb = semanticModel.GetSymbolInfo(invocationArg.Expression).Symbol;
                    if (symb != null && symb.DeclaringSyntaxReferences.Count() == 1 && symb.DeclaringSyntaxReferences.First().GetSyntax().IsKind(SyntaxKind.MethodDeclaration))
                    {
                        var syntaxNode = symb.DeclaringSyntaxReferences.Single().GetSyntax();
                        var methodDeclarationSyntax = (MethodDeclarationSyntax)syntaxNode;
                        var blockSyntax = methodDeclarationSyntax.Body;

                        System.Activities.Statements.Flowchart fc = flowChart.FlowChart(null, symb.Name, (workflowPath) =>
                        {
                            processBlock(workflowPath, blockSyntax);
                        }, false);



                        //paramValues.Add(fc);
                        return null;


                    }
                    else
                    {
                        return WFActivityTypeHelper.CreateArgumentExpression(invocationArg.Expression.ToString(), typeName);
                    }

                }
            }
        }
    }
}
