using CodeConverterCore.Interface;
using CodeConverterCore.Model;
using System.Collections.Generic;

namespace CodeConverterCore.Helper
{
    public static class CodeSteppingHelper
    {
        public static void CheckClassCodeBlocks(ICodeStepperEvents tmpNamespaceStepper, ClassContainer inClass)
        {
            foreach (var tmpField in inClass.FieldList)
            {
                CheckCodeEntry(tmpNamespaceStepper, tmpField);
            }

            //Check Methode Content for requires usings
            foreach (var tmpMethode in inClass.MethodeList)
            {
                if (tmpMethode.Code != null)
                {
                    foreach (var tmpEntry in tmpMethode.Code.CodeEntries)
                    {
                        CheckCodeEntry(tmpNamespaceStepper, tmpEntry);
                    }
                }
            }
        }

        /// <summary>
        /// Call all Codeentries of List<CodeBlock>
        /// </summary>
        public static void CheckCodeBlockList(ICodeStepperEvents inCodeEntryEvents, List<CodeBlock> inCodeBlockList)
        {
            if (inCodeBlockList == null)
            {
                return;
            }
            foreach (var tmpBlock in inCodeBlockList)
            {
                CheckCodeBlock(inCodeEntryEvents, tmpBlock);
            }
        }
        /// <summary>
        /// Call all Codeentries of CodeBlock
        /// </summary>
        public static void CheckCodeBlock(ICodeStepperEvents inCodeEntryEvents, CodeBlock inCodeBlock)
        {
            if (inCodeBlock == null)
            {
                return;
            }
            foreach (var tmpBlockEntry in inCodeBlock.CodeEntries)
            {
                CheckCodeEntry(inCodeEntryEvents, tmpBlockEntry);
            }
        }

        /// <summary>
        /// Handle Code Entry and subEntries
        /// </summary>
        public static void CheckCodeEntry(ICodeStepperEvents inCodeEntryEvents, ICodeEntry inCodeEntry)
        {
            inCodeEntryEvents.CodeEntryStep(inCodeEntry);
            if (inCodeEntry is StatementCode)
            {
                var tmpStatement = inCodeEntry as StatementCode;
                CheckCodeBlock(inCodeEntryEvents, tmpStatement.InnerContent);
                CheckCodeBlockList(inCodeEntryEvents, tmpStatement.StatementCodeBlocks);
            }
            else if (inCodeEntry is ReturnCodeEntry)
            {
                foreach (var tmpEntry in (inCodeEntry as ReturnCodeEntry).CodeEntries)
                {
                    CheckCodeEntry(inCodeEntryEvents, tmpEntry);
                }
            }
            else if (inCodeEntry is NewObjectDeclaration)
            {
                var tmpObjectDecl = (inCodeEntry as NewObjectDeclaration);
                CheckCodeBlockList(inCodeEntryEvents, tmpObjectDecl.ArgumentList);
                CheckCodeEntry(inCodeEntryEvents, tmpObjectDecl.InnerCode);
            }
            else if (inCodeEntry is MethodeCall)
            {
                var tmpMethodeCall = inCodeEntry as MethodeCall;
                CheckCodeBlockList(inCodeEntryEvents, tmpMethodeCall.Parameter);
            }
            else if (inCodeEntry is CodeExpression)
            {
                var tmpExpr = inCodeEntry as CodeExpression;
                CheckCodeBlockList(inCodeEntryEvents, tmpExpr.SubClauseEntries);
            }
            else if (inCodeEntry is CodeBlockContainer)
            {
                var tmpExpr = inCodeEntry as CodeBlockContainer;
                CheckCodeBlock(inCodeEntryEvents, tmpExpr.InnerBlock);
            }
            else if (inCodeEntry is TypeConversion)
            {
                var tmpExpr = inCodeEntry as TypeConversion;
                CheckCodeBlock(inCodeEntryEvents, tmpExpr.PreconversionValue);
            }
            else
            {

            }
        }
    }
}
