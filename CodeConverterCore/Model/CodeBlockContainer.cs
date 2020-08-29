using CodeConverterCore.Interface;

namespace CodeConverterCore.Model
{
    public class CodeBlockContainer : ICodeEntry
    {
        /// <summary>
        /// Name of the Variable to access (filled from Code-Resolver)
        /// </summary>
        public CodeBlock InnerBlock { get; set; } = new CodeBlock();

        public override string ToString()
        {
            return $"{InnerBlock}";
        }
    }
}
