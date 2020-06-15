using CodeConverterCore.Interface;

namespace CodeConverterCore.Model
{
    public class SetFieldWithValue : ICodeEntry
    {
        /// <summary>
        /// Variable that is accessed
        /// </summary>
        public CodeBlock VariableToAccess { get; set; } = new CodeBlock();

        /// <summary>
        /// Value to be set to the Variable
        /// </summary>
        public CodeBlock ValueToSet { get; set; } = new CodeBlock();

        /// <summary>
        /// Override TOString to better debug and Read
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return VariableToAccess.ToString() + "=" + ValueToSet.ToString();
        }
    }
}
