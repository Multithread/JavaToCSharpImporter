using CodeConverterCore.Interface;
using Newtonsoft.Json;
using System.Diagnostics;

namespace CodeConverterCore.Model
{
    [DebuggerDisplay("{Type}: {Name}")]
    public class VariableDeclaration : ICodeEntry, IName
    {
        /// <summary>
        /// Type of the Declaring Variable
        /// </summary>
        [JsonProperty]
        public TypeContainer Type { get; set; }

        /// <summary>
        /// Variablename
        /// </summary>
        public string Name { get; set; }
    }
}
