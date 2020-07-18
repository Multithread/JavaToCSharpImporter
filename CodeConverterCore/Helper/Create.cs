using CodeConverterCore.Enum;
using CodeConverterCore.Interface;
using CodeConverterCore.Model;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace CodeConverterCore.Helper
{
    public static class Create
    {
        /// <summary>
        /// Create ClassContainer
        /// </summary>
        public static ClassContainer AddClass(string inName, string inNamespace = null, params string[] inUsings)
        {
            return new ClassContainer
            {
                Type = new TypeContainer() { Name = inName, Type = new BaseType(inName) },
                Namespace = inNamespace ?? ""
            };
        }

        /// <summary>
        /// Add Field to Class
        /// </summary>
        public static FieldContainer AddField(this ClassContainer inClass, string inFieldName, BaseType inType, string inDefaultValue = null, params FieldAttributes[] inAttributes)
        {
            var tmpFieldContainer = new FieldContainer()
            {
                Name = inFieldName,
                Type = new TypeContainer { Type = inType, Name = inType.Name }
            };
            inClass.FieldList.Add(tmpFieldContainer);
            return tmpFieldContainer;
        }

        /// <summary>
        /// Add Methode to Class
        /// </summary>
        public static MethodeContainer AddMethode(this ClassContainer inClass, string inMethodeName, TypeContainer inReturnType, params FieldContainer[] inFieldContainer)
        {
            var tmpMethode = new MethodeContainer()
            {
                Name = inMethodeName,
                ReturnType = inReturnType,
                Parameter = inFieldContainer.ToList(),
            };
            inClass.AddMethode(tmpMethode);
            return tmpMethode;
        }

        /// <summary>
        /// Create new Variable
        /// </summary>
        public static CodeBlock AddVariable(this CodeBlock inParentBlock, string inName, TypeContainer inType)
        {
            var tmpData = new VariableDeclaration()
            {
                Name = inName,
                Type = inType,
            };
            inParentBlock.CodeEntries.Add(tmpData);
            return inParentBlock;
        }

        /// <summary>
        /// Create new Variable
        /// </summary>
        public static CodeBlock AddReturnStatement(this CodeBlock inParentBlock, string inReturnVariableName)
        {
            var tmpData = new ReturnCodeEntry()
            {
                CodeEntries = new List<ICodeEntry> { new ConstantValue() { Value = inReturnVariableName } }
            };
            inParentBlock.CodeEntries.Add(tmpData);
            return inParentBlock;
        }
        /// <summary>
        /// Create new Variable
        /// </summary>
        public static CodeBlock AddIfStatement(this CodeBlock inParentBlock, CodeBlock inIfBlock, CodeBlock inIfContent, CodeBlock inElseContent = null)
        {
            var tmpStatement = new StatementCode()
            {
                StatementType = StatementTypeEnum.If,
                InnerContent = inIfContent,
                StatementCodeBlocks = new List<CodeBlock> { inIfBlock }
            };
            inParentBlock.CodeEntries.Add(tmpStatement);
            if (inElseContent != null)
            {
                tmpStatement = new StatementCode()
                {
                    StatementType = StatementTypeEnum.Else,
                    InnerContent = inElseContent
                };
                inParentBlock.CodeEntries.Add(tmpStatement);
            }
            return inParentBlock;
        }

        /// <summary>
        /// Set Value to Variable
        /// </summary>
        public static CodeBlock CreateComparisionBlock(string inVar1, VariableOperatorType inOperatorType, string inVar2)
        {
            var tmpCodeBlock = new CodeBlock();
            var tmpExpr = new CodeExpression()
            {
                Manipulator = inOperatorType,
            };
            tmpExpr.SubClauseEntries.Add(new CodeBlock { CodeEntries = new List<ICodeEntry> { new ConstantValue(inVar1) } });
            tmpExpr.SubClauseEntries.Add(new CodeBlock { CodeEntries = new List<ICodeEntry> { new ConstantValue(inVar2) } });
            tmpCodeBlock.CodeEntries.Add(tmpExpr);
            return tmpCodeBlock;
        }

        /// <summary>
        /// Set Value to Variable
        /// </summary>
        public static CodeBlock SetFieldValue(this CodeBlock inParentBlock, ICodeEntry inVariableToAccess, params ICodeEntry[] inDefaultValueCalculation)
        {
            var tmpData = new SetFieldWithValue()
            {
                VariableToAccess = new CodeBlock { CodeEntries = new List<ICodeEntry> { inVariableToAccess } },
                ValueToSet = new CodeBlock { CodeEntries = inDefaultValueCalculation.ToList() }
            };
            inParentBlock.CodeEntries.Add(tmpData);
            return inParentBlock;
        }

        /// <summary>
        /// Set Value to Variable
        /// </summary>
        public static MethodeCall CallMethode(this CodeBlock inParentBlock, string inName, params CodeBlock[] inParamEntries)
        {
            var tmpData = new MethodeCall()
            {
                Name = inName,
                Parameter = inParamEntries.ToList()
            };
            inParentBlock.CodeEntries.Add(tmpData);
            return tmpData;
        }
    }
}
