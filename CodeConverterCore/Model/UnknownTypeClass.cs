using System.Collections.Generic;
using System.Diagnostics;

namespace CodeConverterCore.Model
{
    [DebuggerDisplay("{Type}")]
    public class UnknownTypeClass : ClassContainer
    {
        public UnknownTypeClass(string inName)
        {
            Type = new TypeContainer { Type = new BaseType(inName), Name = inName };
        }

        /// <summary>
        /// Possible Namespaces
        /// </summary>
        public List<string> PossibleNamespace { get; set; } = new List<string>();
    }
}
