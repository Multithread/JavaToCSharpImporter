using JavaToCSharpConverter.Interface;
using System.Collections.Generic;

namespace JavaToCSharpConverter.Model.OOP
{
    public class CodeBlock
    {
        public List<ICodeEntry> CodeEntries { get; set; } = new List<ICodeEntry>();

        /// <summary>
        /// Override TOString to better debug and Read
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return string.Join(" ", CodeEntries);
        }
    }
}
