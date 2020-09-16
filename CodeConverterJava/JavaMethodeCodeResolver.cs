using Antlr4.Runtime.Tree;
using CodeConverterCore.Enum;
using CodeConverterCore.Helper;
using CodeConverterCore.Interface;
using CodeConverterCore.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using static CodeConverterJava.Model.JavaParser;

namespace CodeConverterJava.Model
{
    public class JavaMethodeCodeResolver : IResolveMethodeContentToIL
    {
        /// <summary>
        /// ParentClass Info
        /// </summary>
        public ClassContainer ParentClass { get; set; }

        public void Resolve(MethodeContainer inMethodeContainer)
        {
            if (inMethodeContainer.AntlrCode == null)
            {
                return;
            }
            var tmpCodeBlockStack = new Stack<CodeBlock>();
            CodeBlock tmpCurrentCodeBlock = null;
            foreach (var tmpElement in inMethodeContainer.AntlrCode.GetChildren())
            {
                var tmpType = tmpElement.GetType().Name;
                if (tmpElement is BlockContext)
                {
                    var tmpBlock = tmpElement as BlockContext;
                    foreach (var tmpBlockChild in tmpBlock.GetChildren())
                    {
                        var tmpBlockType = tmpBlockChild.GetType().Name;
                        if (tmpBlockChild is TerminalNodeImpl)
                        {
                            var tmpBlockText = tmpBlockChild.GetText();
                            if (tmpBlockText == "{")
                            {
                                tmpCurrentCodeBlock = new CodeBlock();
                                tmpCodeBlockStack.Push(tmpCurrentCodeBlock);
                                if (tmpCodeBlockStack.Count == 1)
                                {
                                    inMethodeContainer.Code = tmpCurrentCodeBlock;
                                }
                            }
                            else if (tmpBlockText == "}")
                            {
                                tmpCurrentCodeBlock = tmpCodeBlockStack.Pop();
                            }
                            else if (tmpBlockText.StartsWith("/"))
                            {
                            }
                            else
                            {
                                throw new NotImplementedException("Not done yet");
                            }
                        }
                        else if (tmpBlockChild is BlockStatementContext)
                        {
                            HandloBlockStatementContent(tmpCurrentCodeBlock, tmpBlockChild as BlockStatementContext);
                        }
                        else
                        {
                            throw new NotImplementedException("Not done yet");
                        }
                    }
                }
                else if (tmpElement is BlockStatementContext)
                {
                    if (tmpCurrentCodeBlock == null)
                    {
                        tmpCurrentCodeBlock = new CodeBlock();
                        tmpCodeBlockStack.Push(tmpCurrentCodeBlock);
                        inMethodeContainer.Code = tmpCurrentCodeBlock;
                    }
                    HandloBlockStatementContent(tmpCurrentCodeBlock, tmpElement as BlockStatementContext);
                }
                else if (tmpElement is TerminalNodeImpl)
                {
                }
                else
                {
                    throw new NotImplementedException("Not done yet");
                }
            }
        }

        private void HandloBlockStatementContent(CodeBlock inCodeBlock, BlockStatementContext inBlockStatement)
        {
            var tmpVar = inBlockStatement.localVariableDeclaration();
            if (tmpVar != null)
            {
                HandleLocalVariableDeclarationContext(inCodeBlock, tmpVar);
            }
            var tmpType = inBlockStatement.localTypeDeclaration();
            if (tmpType != null)
            {
                throw new NotImplementedException("Not done yet");
            }
            var tmpStatement = inBlockStatement.statement();
            if (tmpStatement != null)
            {
                HandleBlockStatementStatement(inCodeBlock, inBlockStatement.statement());
            }

        }

        private void HandleLocalVariableDeclarationContext(CodeBlock inCodeBlock, LocalVariableDeclarationContext tmpVar)
        {
            var tmpVarDeclaration = new VariableDeclaration();
            if (tmpVar.typeType() != null)
            {
                tmpVarDeclaration.Type = JavaAntlrClassLoader.GetTypeContainer(tmpVar.typeType());
            }
            if (tmpVar.variableDeclarators() != null)
            {
                foreach (var tmpVariableDeclaration in tmpVar.variableDeclarators().variableDeclarator())
                {
                    var tmpVarDec = new VariableDeclaration()
                    {
                        Type = tmpVarDeclaration.Type,
                        Name = tmpVariableDeclaration.variableDeclaratorId().GetText(),
                    };
                    inCodeBlock.CodeEntries.Add(tmpVarDec);
                    if (tmpVariableDeclaration.variableInitializer() != null)
                    {
                        HandleArrayInizializer(inCodeBlock, tmpVariableDeclaration.variableInitializer().arrayInitializer(), tmpVarDec);
                        HandleExpressionContext(inCodeBlock, tmpVariableDeclaration.variableInitializer().expression(), tmpVarDec);
                    }
                }
            }
            if (tmpVar.variableModifier().Length > 0)
            {

                throw new NotImplementedException("Not done yet");
            }
        }

        /// <summary>
        /// Handling an Array Inizializer block
        /// I'm unsure why this is not an ExpressionCOntext, but whatever
        /// </summary>
        /// <param name="inCodeBlock"></param>
        /// <param name="inBlockStatement"></param>
        private void HandleArrayInizializer(CodeBlock inCodeBlock, ArrayInitializerContext inBlockStatement, VariableDeclaration inVariable)
        {
            if (inBlockStatement == null)
            {
                return;
            }
            var b = 0;
            throw new NotImplementedException("Not done yet");
        }


        /// <summary>
        /// Handling an Array Inizializer block
        /// I'm unsure why this is not an ExpressionCOntext, but whatever
        /// </summary>
        /// <param name="inCodeBlock"></param>
        /// <param name="inBlockStatement"></param>
        private void HandleBlockStatementStatement(CodeBlock inParentCodeBlock, StatementContext inStatement)
        {
            if (inStatement.expression().Length > 1)
            {
                throw new NotImplementedException("Not done yet");
            }
            var tmpStatement = new StatementCode();
            if (inStatement.IF() != null)
            {
                if (inStatement.statement().Length > 2)
                {
                    throw new NotImplementedException("statement.statement length bigger than 1");
                }
                tmpStatement.StatementType = StatementTypeEnum.If;
                tmpStatement.StatementCodeBlocks = new List<CodeBlock>() { new CodeBlock() };
                HandleExpressionContext(tmpStatement.StatementCodeBlocks[0], inStatement.parExpression().expression(), null);
                tmpStatement.InnerContent = new CodeBlock();
                HandleBlockStatementStatement(tmpStatement.InnerContent, inStatement.statement()[0]);

                if (inStatement.ELSE() != null)
                {
                    inParentCodeBlock.CodeEntries.Add(tmpStatement);
                    tmpStatement = new StatementCode();

                    tmpStatement.StatementType = StatementTypeEnum.Else;
                    tmpStatement.StatementCodeBlocks = new List<CodeBlock>() { new CodeBlock() };
                    HandleExpressionContext(tmpStatement.StatementCodeBlocks[0], inStatement.parExpression().expression(), null);
                    tmpStatement.InnerContent = new CodeBlock();
                    HandleBlockStatementStatement(tmpStatement.InnerContent, inStatement.statement().Last());
                }
            }
            else if (inStatement.ASSERT() != null)
            {
                tmpStatement.StatementType = StatementTypeEnum.Assert;
                tmpStatement.StatementCodeBlocks = new List<CodeBlock>() { };
                foreach (var tmpStatementExpression in inStatement.expression())
                {
                    var tmpBlock = new CodeBlock();
                    HandleExpressionContext(tmpBlock, tmpStatementExpression);
                    tmpStatement.StatementCodeBlocks.Add(tmpBlock);
                }
            }
            else if (inStatement.IDENTIFIER() != null)
            {
                throw new NotImplementedException("Not done yet");
            }
            else if (inStatement.RETURN() != null)
            {
                var tmpReturnCodeEntry = new ReturnCodeEntry();
                HandleExpressionContext(tmpReturnCodeEntry, inStatement.expression()[0], null);
                inParentCodeBlock.CodeEntries.Add(tmpReturnCodeEntry);
                return;
            }
            else if (inStatement.SEMI() != null)
            {
                //Semicolon, so it is a simple Statement
                //tmpStatement.StatementCodeBlocks = new List<CodeBlock>() { new CodeBlock() };
                if (inStatement.expression()[0].GetText() + ";" != inStatement.GetText())
                {
                    throw new NotImplementedException("Unhandlet Statement in Semi");
                }

                HandleExpressionContext(inParentCodeBlock, inStatement.expression()[0], null);
                return;
            }
            else if (inStatement.block() != null)
            {
                foreach (var tmpCode in inStatement.block().blockStatement())
                {
                    HandloBlockStatementContent(inParentCodeBlock, tmpCode);
                }
                return;
            }
            else if (inStatement.forControl() != null)
            {
                var tmpForControl = inStatement.forControl();
                tmpStatement.StatementType = StatementTypeEnum.For;
                tmpStatement.StatementCodeBlocks = new List<CodeBlock>() { new CodeBlock(), new CodeBlock(), new CodeBlock() };
                tmpStatement.InnerContent = new CodeBlock();
                HandleBlockStatementStatement(tmpStatement.InnerContent, inStatement.statement()[0]);

                if (tmpForControl.forInit() != null)
                {
                    if (tmpForControl.forInit().localVariableDeclaration() != null)
                    {
                        HandleLocalVariableDeclarationContext(tmpStatement.StatementCodeBlocks[0], tmpForControl.forInit().localVariableDeclaration());
                    }
                    if (tmpForControl.forInit().expressionList() != null)
                    {
                        foreach (var tmpExpr in tmpForControl.forInit().expressionList().expression())
                        {
                            HandleExpressionContext(tmpStatement.StatementCodeBlocks[0], tmpExpr);
                        }
                    }
                }
                if (tmpForControl.expression() != null)
                {
                    HandleExpressionContext(tmpStatement.StatementCodeBlocks[1], tmpForControl.expression());
                }
                if (tmpForControl.expressionList() != null)
                {
                    foreach (var tmpExpr in tmpForControl.expressionList().expression())
                    {
                        HandleExpressionContext(tmpStatement.StatementCodeBlocks[2], tmpExpr);
                    }
                }
                if (tmpForControl.enhancedForControl() != null)
                {
                    throw new NotImplementedException("for Inline Variable Declaration missing yet");
                    HandleExpressionContext(tmpStatement.StatementCodeBlocks[1], tmpForControl.expression());
                }

            }
            else
            {
                throw new NotImplementedException("Not done yet");
            }

            if (inStatement.statement().Length > 0)
            {
                var tmpinnercount = (inStatement.ELSE() != null ? 1 : 0)
                    + (inStatement.IF() != null ? 1 : 0)
                    + (inStatement.FOR() != null ? 1 : 0);

                if (inStatement.statement().Length != tmpinnercount)
                {
                    throw new NotImplementedException("Statement inner Statement length not matching");
                }
            }

            inParentCodeBlock.CodeEntries.Add(tmpStatement);
        }

        /// <summary>
        /// Handling of an Expression Block
        /// </summary>
        /// <param name="inCodeBlock"></param>
        /// <param name="inBlockStatement"></param>
        public void HandleExpressionContext(CodeBlock inCodeBlock, ExpressionContext inBlockStatement, VariableDeclaration inVariable = null)
        {
            if (inBlockStatement == null)
            {
                return;
            }

            //if (inBlockStatement.IDENTIFIER() != null)
            //{
            //    inCodeBlock.CodeEntries.Add(new ConstantValue { Value = inBlockStatement.IDENTIFIER().GetText() });
            //}
            //else if (inBlockStatement.THIS() != null)
            //{
            //    throw new NotImplementedException("Not done yet");
            //}
            //else if (inBlockStatement.NEW() != null)
            //{
            //    throw new NotImplementedException("Not done yet");
            //}
            //else if (inBlockStatement.SUPER() != null)
            //{
            //    throw new NotImplementedException("Not done yet");
            //}
            //else if (inBlockStatement.INSTANCEOF() != null)
            //{
            //    throw new NotImplementedException("Not done yet");
            //}
            //else
            {
                if (inBlockStatement.primary() != null)
                {
                    //Primary Value analyse type
                    var tmpPrimary = inBlockStatement.primary();
                    var tmpPrimaryAsText = tmpPrimary.GetText();

                    if (tmpPrimary.expression() != null)
                    {
                        var tmpCodeBlock = new CodeBlockContainer();
                        HandleExpressionContext(tmpCodeBlock.InnerBlock, tmpPrimary.expression(), inVariable);
                        inCodeBlock.CodeEntries.Add(tmpCodeBlock);
                    }
                    else if (tmpPrimary.literal() != null)
                    {
                        inCodeBlock.CodeEntries.Add(new ConstantValue { Value = tmpPrimaryAsText });
                    }
                    else if (tmpPrimary.typeTypeOrVoid() != null)
                    {
                        throw new NotImplementedException("Not done yet");
                    }
                    else if (tmpPrimary.nonWildcardTypeArguments() != null)
                    {
                        throw new NotImplementedException("Not done yet");
                    }
                    else if (tmpPrimary.explicitGenericInvocationSuffix() != null)
                    {
                        throw new NotImplementedException("Not done yet");
                    }
                    else if (tmpPrimary.arguments() != null)
                    {
                        throw new NotImplementedException("Not done yet");
                    }
                    else
                    {
                        inCodeBlock.CodeEntries.Add(new ConstantValue { Value = tmpPrimaryAsText });
                    }
                }
                else if (inBlockStatement.expression().Length == 1
                    && inBlockStatement.typeType() != null)
                {
                    //Type Conversion
                    var tmpInfo = inBlockStatement.expression();
                    var tmpConverter = new TypeConversion();
                    tmpConverter.PreconversionValue = new CodeBlock();
                    HandleExpressionContext(tmpConverter.PreconversionValue, tmpInfo[0]);

                    var tmpType = inBlockStatement.typeType();
                    tmpConverter.Type = JavaAntlrClassLoader.GetTypeContainer(tmpType);
                    inCodeBlock.CodeEntries.Add(tmpConverter);
                }
                else if (inBlockStatement.expression().Length == 2
                    && inBlockStatement.children[1].GetText() != "=")
                {
                    var tmpCodeExpression = new CodeExpression
                    {
                        Manipulator = JavaStaticInfo.GetManipulator(string.Join("", inBlockStatement.children
                        .Where(inItem => inItem is ITerminalNode)
                        .Select(inItem => inItem.GetText())))
                    };
                    var tmpCodeBlock = new CodeBlock();
                    HandleExpressionContext(tmpCodeBlock, inBlockStatement.expression()[0], inVariable);
                    tmpCodeExpression.SubClauseEntries.Add(tmpCodeBlock);
                    tmpCodeBlock = new CodeBlock();//Second Code Block
                    HandleExpressionContext(tmpCodeBlock, inBlockStatement.expression()[1], inVariable);
                    tmpCodeExpression.SubClauseEntries.Add(tmpCodeBlock);

                    inCodeBlock.CodeEntries.Add(tmpCodeExpression);

                }
                else
                {
                    var tmpChildList = inBlockStatement.children;
                    if (tmpChildList.Count > 2)
                    {
                        var tmpSecondChildText = tmpChildList[1].GetText();
                        if (tmpSecondChildText == "=")
                        {
                            //SetVariable with Value
                            var tmpVarSetter = new SetFieldWithValue();
                            HandleExpressionContext(tmpVarSetter.VariableToAccess, tmpChildList[0] as ExpressionContext, inVariable);
                            HandleExpressionContext(tmpVarSetter.ValueToSet, tmpChildList[2] as ExpressionContext, inVariable);
                            inCodeBlock.CodeEntries.Add(tmpVarSetter);
                        }
                        else if (JavaStaticInfo.VariableOperators.ContainsKey(tmpSecondChildText))
                        {
                            var tmpCodeExpression = new CodeExpression
                            {
                                Manipulator = JavaStaticInfo.GetManipulator(tmpSecondChildText)
                            };
                            var tmpCodeBlock = new CodeBlock();
                            HandleExpressionContext(tmpCodeBlock, tmpChildList[0] as ExpressionContext, inVariable);
                            tmpCodeExpression.SubClauseEntries.Add(tmpCodeBlock);
                            tmpCodeBlock = new CodeBlock();//Second Code Block
                            HandleExpressionContext(tmpCodeBlock, tmpChildList[2] as ExpressionContext, inVariable);
                            tmpCodeExpression.SubClauseEntries.Add(tmpCodeBlock);

                            inCodeBlock.CodeEntries.Add(tmpCodeExpression);
                        }
                        //Multi Part Property Access
                        else if (tmpSecondChildText == ".")
                        {
                            VariableAccess tmpParent = null;
                            for (var tmpI = 0; tmpI < tmpChildList.Count; tmpI += 2)
                            {
                                var tmpChild = tmpChildList[tmpI];
                                var tmpAccess = new VariableAccess();
                                if (tmpChild is ExpressionContext)
                                {
                                    var tmpCodeBlock = new CodeBlock();
                                    HandleExpressionContext(tmpCodeBlock, tmpChild as ExpressionContext, null);
                                    tmpAccess.Access = tmpCodeBlock.CodeEntries[0];
                                }
                                else if (tmpChild is MethodCallContext)
                                {
                                    var tmpResult = HandleMethodeCall(tmpChild as MethodCallContext);
                                    tmpAccess.Access = tmpResult;
                                }
                                else if (tmpChild is TerminalNodeImpl)
                                {
                                    var tmpChildText = tmpChild.GetText();
                                    if (tmpChildText == ".") { }
                                    else if (RegexHelper.WordCheck.IsMatch(tmpChildText))
                                    {
                                        tmpAccess.Access = new ConstantValue(tmpChildText);
                                    }
                                    else
                                    {
                                        throw new NotImplementedException("Not done yet");
                                    }
                                }
                                else
                                {
                                    throw new NotImplementedException("Not done yet");
                                }
                                if (tmpParent != null)
                                {
                                    tmpParent.Child = tmpAccess;
                                }
                                else
                                {
                                    inCodeBlock.CodeEntries.Add(tmpAccess);
                                }
                                tmpParent = tmpAccess;
                            }
                        }
                        else if (tmpSecondChildText == "?")
                        {
                            //Implement Elvis
                            var tmpStatement = new StatementCode
                            {
                                StatementType = StatementTypeEnum.Elvis,
                                StatementCodeBlocks = new List<CodeBlock>()
                            };
                            //Boolean query
                            var tmpCodeBlock = new CodeBlock();
                            HandleExpressionContext(tmpCodeBlock, tmpChildList[0] as ExpressionContext, inVariable);
                            tmpStatement.StatementCodeBlocks.Add(tmpCodeBlock);
                            //First Result
                            tmpCodeBlock = new CodeBlock();
                            HandleExpressionContext(tmpCodeBlock, tmpChildList[2] as ExpressionContext, inVariable);
                            tmpStatement.StatementCodeBlocks.Add(tmpCodeBlock);
                            //Second Result
                            tmpCodeBlock = new CodeBlock();
                            HandleExpressionContext(tmpCodeBlock, tmpChildList[4] as ExpressionContext, inVariable);
                            tmpStatement.StatementCodeBlocks.Add(tmpCodeBlock);

                            inCodeBlock.CodeEntries.Add(tmpStatement);
                        }
                        else
                        {
                            throw new NotImplementedException("Not done yet");
                        }
                    }
                    else if (tmpChildList.Count == 1)
                    {
                        var tmpValue = tmpChildList[0] as MethodCallContext;
                        MethodeCall tmpMethodeCall = HandleMethodeCall(tmpValue);
                        inCodeBlock.CodeEntries.Add(tmpMethodeCall);
                    }
                    else if (tmpChildList.Count == 2
                        && tmpChildList[1] is ExpressionContext)
                    {
                        if (tmpChildList[0].GetText() != "-")
                        {
                            throw new NotImplementedException("Not done yet");
                        }
                        HandleExpressionContext(inCodeBlock, tmpChildList[1] as ExpressionContext, inVariable);
                        (inCodeBlock.CodeEntries.Last() as ConstantValue).Value = "-" + (inCodeBlock.CodeEntries.Last() as ConstantValue).Value;
                    }
                    else if (tmpChildList.Count == 2
                        && tmpChildList[0] is ExpressionContext)
                    {
                        if (tmpChildList[1].GetText() != "--" && tmpChildList[1].GetText() != "++")
                        {
                            throw new NotImplementedException("Not done yet");
                        }
                        var tmpCodeExpression = new CodeExpression
                        {
                            Manipulator = JavaStaticInfo.GetManipulator(tmpChildList[1].GetText())
                        };
                        tmpCodeExpression.SubClauseEntries = new List<CodeBlock> { new CodeBlock() };
                        HandleExpressionContext(tmpCodeExpression.SubClauseEntries[0], tmpChildList[0] as ExpressionContext, inVariable);
                        inCodeBlock.CodeEntries.Add(tmpCodeExpression);
                    }
                    else if (tmpChildList.Count == 2)
                    {
                        if (tmpChildList[0].GetText() != "new")
                        {
                            throw new NotImplementedException("Not done yet");
                        }
                        inCodeBlock.CodeEntries.Add(HandleCreatorContext(tmpChildList[1] as CreatorContext, inVariable));
                    }
                    else
                    {
                        throw new NotImplementedException("Not done yet");
                    }
                }
            }

        }

        private NewObjectDeclaration HandleCreatorContext(CreatorContext inContext, VariableDeclaration inVariableDeclaration)
        {
            var tmpEntry = new NewObjectDeclaration();
            ConstantValue tmpValue = null;
            if (inContext.createdName() != null)
            {
                tmpValue = new ConstantValue();
                tmpValue.Type = new TypeContainer() { Name = inContext.createdName().GetText() };
                tmpEntry.InnerCode = tmpValue;
            }
            if (inContext.nonWildcardTypeArguments() != null)
            {
                throw new NotImplementedException("Not done yet");
            }
            if (inContext.classCreatorRest() != null)
            {
                var tmpClass = inContext.classCreatorRest();
                if (tmpClass.arguments()?.expressionList() != null)
                {
                    tmpEntry.ArgumentList = new List<CodeBlock>();
                    foreach (var tmpExpression in tmpClass.arguments().expressionList().expression())
                    {
                        var tmpBlock = new CodeBlock();
                        HandleExpressionContext(tmpBlock, tmpExpression);
                        tmpEntry.ArgumentList.Add(tmpBlock);
                    }
                }
                if (tmpClass.classBody() != null)
                {
                    if (inVariableDeclaration == null)
                    {
                        throw new NotImplementedException("Not done yet");
                    }
                    var tmpClassContainer = new ClassContainer()
                    {
                        Namespace = "",
                        ModifierList = ParentClass.ModifierList.Where(inItem => inItem != "abstract").ToList(),
                    };

                    JavaAntlrClassLoader.ManageClassBodyContext(tmpClassContainer, "", tmpClass.classBody());
                    tmpClassContainer.Type = new TypeContainer(ParentClass.Name + "_" + ParentClass.InnerClasses.Count);
                    tmpClassContainer.InterfaceList.Add(inVariableDeclaration.Type);
                    ParentClass.InnerClasses.Add(tmpClassContainer);

                    tmpValue = new ConstantValue();
                    tmpValue.Type = new TypeContainer() { Name = tmpClassContainer.Name };
                    tmpEntry.InnerCode = tmpValue;

                }
            }
            if (inContext.arrayCreatorRest() != null)
            {
                tmpValue.Type.IsArray = true;
                var tmpBlock = new CodeBlock();
                HandleExpressionContext(tmpBlock, inContext.arrayCreatorRest().expression()[0]);
                var tmpArrayInizialiser = tmpBlock.CodeEntries[0];
                tmpValue.Type.ArrayInizialiser = tmpArrayInizialiser;
            }
            return tmpEntry;
        }

        private MethodeCall HandleMethodeCall(MethodCallContext inMethodeCallContext)
        {
            var tmpMethodeCall = new MethodeCall()
            {
                Name = (inMethodeCallContext.SUPER()?? inMethodeCallContext.THIS() ?? inMethodeCallContext.IDENTIFIER()).GetText()
            };
            if (inMethodeCallContext.expressionList() != null)
            {
                foreach (ExpressionContext tmpExpression in inMethodeCallContext.expressionList().expression())
                {
                    //Handle expression Context for Methode Call Parameter
                    var tmpCodeParam = new CodeBlock();
                    HandleExpressionContext(tmpCodeParam, tmpExpression, null);
                    tmpMethodeCall.Parameter.Add(tmpCodeParam);
                }
            }

            return tmpMethodeCall;
        }
    }
}
