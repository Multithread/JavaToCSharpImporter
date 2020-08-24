using CodeConverterCore.Interface;
using System.Collections.Generic;

namespace CodeConverterCore.Model
{
    public class MethodeCall : ICodeEntry
    {
        public string Name
        {
            get => MethodeLink?.Name ?? _name;
            set => _name = value;
        }
        private string _name;

        /// <summary>
        /// Methode Return Type
        /// </summary>
        public MethodeContainer MethodeLink { get; set; }

        /// <summary>
        /// Methode Parameter
        /// </summary>
        public List<CodeBlock> Parameter { get; set; } = new List<CodeBlock>();

        /// <summary>
        /// Return Constant as Type with Value
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return $" {Name} ({string.Join(",", Parameter)})";
        }
    }
}
