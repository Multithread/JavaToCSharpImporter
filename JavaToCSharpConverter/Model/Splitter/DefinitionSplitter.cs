using JavaToCSharpConverter.Helper;
using JavaToCSharpConverter.Interface;

namespace JavaToCSharpConverter.Model.Splitter
{
    /// <summary>
    /// 
    /// </summary>
    public class DefinitionSplitter : CodeSplitterEvents
    {
        public bool AllowInside { get; set; } = false;
        public override object CurrentCharacter(char inChar)
        {
            if (inChar == ' ')
            {
                if (GetCurrentStack().Count < 1)
                {
                    return CodeResultType.None;
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
