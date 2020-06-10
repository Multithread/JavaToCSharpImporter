using JavaToCSharpConverter.Interface;
using System.Collections.Generic;

namespace JavaToCSharpConverter.Model.OOP
{
    public class CodeBlock
    {
        public List<ICodeEntry> CodeEntries { get; set; } = new List<ICodeEntry>();
    }
}
