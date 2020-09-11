using CodeConverterCore.Enum;
using CodeConverterCore.Interface;
using System.Collections.Generic;
using System.Linq;

namespace CodeConverterCore.Model
{
    public class StatementCode : ICodeEntry
    {
        public StatementCode()
        {

        }

        /// <summary>
        /// Type of Statement
        /// </summary>
        public StatementTypeEnum StatementType { get; set; }

        /// <summary>
        /// Content inside the Statement
        /// </summary>
        public CodeBlock InnerContent { get; set; }

        /// <summary>
        /// Code that is Required by the statement (aka if expression or for(;;) information)
        /// </summary>
        public List<CodeBlock> StatementCodeBlocks { get; set; }

        /// <summary>
        /// Override TOString to better debug and Read
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            switch (StatementType)
            {
                case StatementTypeEnum.Elvis:
                    return $" {StatementCodeBlocks[0]} ? {StatementCodeBlocks[1]} : {StatementCodeBlocks[2]};";
            }
            return $"{StatementType} {StatementCodeBlocks.FirstOrDefault()}{{{InnerContent}}}";
        }
    }
}
