using JavaToCSharpConverter.Helper;
using JavaToCSharpConverter.Interface;

namespace JavaToCSharpConverter.Model.Splitter
{
    /// <summary>
    /// Split where a Spezific End Char is found
    /// </summary>
    public class EndCharSplitter : CodeSplitterEvents
    {
        public bool AllowInside { get; set; } = false;
        public string EndChar { get; set; }

        public override object CurrentCharacter(char inChar)
        {
            return null;
        }

        public override object Ended(Checker inElement)
        {
            if (!AllowInside && GetCurrentStack().Count > 1)
            {
                return null;
            }
            if (inElement.End == EndChar)
            {
                return CodeResultType.InCurlyBracket;
            }
            return null;
        }

        public override object Started(Checker inElement)
        {
            return null;
        }
    }
}
