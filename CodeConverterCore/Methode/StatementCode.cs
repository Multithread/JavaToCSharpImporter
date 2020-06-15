using JavaToCSharpConverter.Interface;
using System.Collections.Generic;

namespace JavaToCSharpConverter.Model.OOP
{
    public class StatementCode : ICodeEntry
    {
        /// <summary>
        /// Type of Statement
        /// </summary>
        public StatementTypeEnum StatementType { get; set; }

        public CodeBlock InnerContent { get; set; }

        /// <summary>
        /// Code that is Required by the statement (aka if expression and for information)
        /// </summary>
        public List<CodeBlock> StatementCodeBlocks { get; set; }

        /// <summary>
        /// Override TOString to better debug and Read
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return StatementType.ToString();
        }
    }
}
