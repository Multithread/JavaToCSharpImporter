using Antlr4.Runtime.Tree;
using JavaToCSharpConverter.Helper;
using JavaToCSharpConverter.Interface;
using JavaToCSharpConverter.Model.OOP;
using System;
using System.Collections.Generic;
using static JavaToCSharpConverter.Model.Java.JavaParser;

namespace JavaToCSharpConverter.Model.Java
{
    public class JavaMethodeCodeResolver : IResolveMethodeContentToIL
    {
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
                var tmpVarDeclaration = new VariableDeclaration();
                if (tmpVar.typeType() != null)
                {
                    tmpVarDeclaration.Type = tmpVar.typeType().GetText();
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

                        HandleArrayInizializer(inCodeBlock, tmpVariableDeclaration.variableInitializer().arrayInitializer());
                        HandleExpressionContext(inCodeBlock, tmpVariableDeclaration.variableInitializer().expression());
                    }
                }
                if (tmpVar.variableModifier() != null)
                {

                }
            }
            var tmpType = inBlockStatement.localTypeDeclaration();
            if (tmpType != null)
            {

            }
            var tmpStatement = inBlockStatement.statement();
            if (tmpStatement != null)
            {

            }

        }

        /// <summary>
        /// Handling an Array Inizializer block
        /// I'm unsure why this is not an ExpressionCOntext, but whatever
        /// </summary>
        /// <param name="inCodeBlock"></param>
        /// <param name="inBlockStatement"></param>
        private void HandleArrayInizializer(CodeBlock inCodeBlock, ArrayInitializerContext inBlockStatement)
        {
            if (inBlockStatement == null)
            {
                return;
            }
        }

        /// <summary>
        /// Handling of an Expression Block
        /// </summary>
        /// <param name="inCodeBlock"></param>
        /// <param name="inBlockStatement"></param>
        private void HandleExpressionContext(CodeBlock inCodeBlock, ExpressionContext inBlockStatement)
        {
            if (inBlockStatement == null)
            {
                return;
            }
        }
    }
}
