using CodeConverterCore.Interface;
using CodeConverterCore.Enum;
using System.Collections.Generic;


namespace CodeConverterCore.Model
{
    public class CodeExpression : ICodeEntry
    {
        /// <summary>
        /// What sort of Manipulation is done?
        /// </summary>
        public VariableManipulatorType Manipulator { get; set; }

        public List<CodeBlock> SubClauseEntries { get; set; } = new List<CodeBlock>();

        /// <summary>
        /// Override TOString to better debug and Read
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return $"({string.Join($" {Manipulator} ", SubClauseEntries)})";
        }
    }
}
