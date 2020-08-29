using CodeConverterCore.Enum;
using CodeConverterCore.Interface;
using CodeConverterCore.Model;

namespace JavaToCSharpConverter.Model
{
    /// <summary>
    /// Used to check the Elvis-operator for C# problems with structs and nullable values
    /// </summary>
    public class ElvisStatementCodeStepper : ICodeStepperEvents
    {
        public ElvisStatementCodeStepper(ClassContainer inCurrentClass)
        {
            _currentClass = inCurrentClass;
        }
        private ClassContainer _currentClass;

        public void CodeEntryStep(ICodeEntry inCodeEntry)
        {
            if (inCodeEntry is StatementCode)
            {
                var tmpStatement = inCodeEntry as StatementCode;
                if (tmpStatement.StatementType == StatementTypeEnum.Elvis)
                {
                    if (IsCodeBlockNull(tmpStatement.StatementCodeBlocks[1]))
                    {

                    }
                    else if (IsCodeBlockNull(tmpStatement.StatementCodeBlocks[2]))
                    {

                    }
                }
            }
        }

        private bool IsCodeBlockNull(CodeBlock inBlock)
        {
            if(((inBlock?.CodeEntries[0] as ConstantValue)?.Value as TypeContainer)?.Name == "null")
            {
                return true;
            }
            return false;
        }
    }
}
