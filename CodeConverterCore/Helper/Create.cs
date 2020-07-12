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
            var tmpFieldContainer=new FieldContainer()
            {
                Name = inFieldName,
                DefaultValue = inDefaultValue,
                Type = new TypeContainer { Type = inType, Name = inType.Name }
            };
            inClass.FieldList.Add(tmpFieldContainer);
            return tmpFieldContainer;
        }

        /// <summary>
        /// Add Methode to Class
        /// </summary>
        public static ClassContainer AddMethode(this ClassContainer inClass, string inMethodeName, TypeContainer inReturnType, params FieldContainer[] inFieldContainer)
        {
            inClass.AddMethode(new MethodeContainer()
            {
                Name = inMethodeName,
                ReturnType = inReturnType,
                Parameter = inFieldContainer.ToList(),
            });
            return inClass;
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
    }
}
