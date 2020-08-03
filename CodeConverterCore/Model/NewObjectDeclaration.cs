using CodeConverterCore.Interface;
using System.Collections.Generic;

namespace CodeConverterCore.Model
{
    public class NewObjectDeclaration : ICodeEntry
    {
        public ICodeEntry InnerCode { get; set; }

        public List<CodeBlock> ArgumentList { get; set; }
    }
}
