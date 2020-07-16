using CodeConverterCore.Interface;
using Newtonsoft.Json;

namespace CodeConverterCore.Model
{
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
