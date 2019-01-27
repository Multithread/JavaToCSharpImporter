using JavaToCSharpConverter.Helper;
using JavaToCSharpConverter.Interface;

namespace JavaToCSharpConverter.Model.Splitter
{
    /// <summary>
    /// Split where a Spezific End Char is found
    /// </summary>
    public class ClassInterfaceSplitter : CodeSplitterEvents
    {
        public override object CurrentCharacter(char inChar)
        {
            if (inChar == ',')
            {
                if (GetCurrentStack().Count == 0)
                {
                    return CodeResultType.LineEnd;
                }
            }
            return null;
        }

        public override object Ended(Checker inElement)
        {
            return null;
        }

        public override object Started(Checker inElement)
        {
            return null;
        }
    }
}
