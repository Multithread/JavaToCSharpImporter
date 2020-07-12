using CodeConverterCore.Interface;

namespace CodeConverterCore.Model
{
    /// <summary>
    /// Return Code Entry for handling of Return elements
    /// </summary>
    public class ReturnCodeEntry : CodeBlock, ICodeEntry
    {
        /// <summary>
        /// Yield Return?
        /// </summary>
        public bool IsYield { get; set; }
    }
}
