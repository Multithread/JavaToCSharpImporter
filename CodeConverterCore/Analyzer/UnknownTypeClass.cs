using CodeConverterCore.Model;
using System.Collections.Generic;
using System.Diagnostics;

namespace CodeConverterCore.Analyzer
{
    [DebuggerDisplay("{Type}")]
    public class UnknownTypeClass
    {
        public UnknownTypeClass(string inName)
        {
            Type = new BaseType(inName);
        }
        public BaseType Type { get; set; }

        /// <summary>
        /// List of Fields of an unknonw Type
        /// </summary>
        public List<FieldContainer> FieldList { get; set; } = new List<FieldContainer>();

        /// <summary>
        /// List of Methods of unkown Class
        /// </summary>
        public List<MethodeContainer> MethodeList { get; set; } = new List<MethodeContainer>();

        /// <summary>
        /// Possible Namespaces
        /// </summary>
        public List<string> PossibleNamespace { get; set; } = new List<string>();
    }
}
