using CodeConverterCore.Interface;
using CodeConverterCore.Model;
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
        public static ClassContainer AddField(this ClassContainer inClass, string inFieldName, BaseType inType, string inDefaultValue = null, params FieldAttributes[] inAttributes)
        {
            inClass.FieldList.Add(new FieldContainer()
            {
                Name = inFieldName,
                DefaultValue = inDefaultValue,
                Type = new TypeContainer { Type = inType, Name = inType.Name }
            });
            return inClass;
        }

        /// <summary>
        /// Add Methode to Class
        /// </summary>
        public static ClassContainer AddMethode(this ClassContainer inClass, string inMethodeName, BaseType inReturnType, params ICodeEntry[] inCode)
        {
            inClass.MethodeList.Add(new MethodeContainer()
            {
                Name = inMethodeName,
                ReturnType = new TypeContainer { Type = inReturnType, Name = inReturnType.Name },
                Code = new CodeBlock
                {
                    CodeEntries = inCode.ToList()
                }
            }); ;
            return inClass;
        }


    }
}
