using JavaToCSharpConverter.Helper;
using JavaToCSharpConverter.Interface;

namespace JavaToCSharpConverter.Model.Splitter
{
    public class HeaderDefinitionSplitter : CodeSplitterEvents
    {
        public bool AllowInside { get; set; } = false;
        public override object CurrentCharacter(char inChar)
        {
            if (inChar == ',')
            {
                if (GetCurrentStack().Count <2)
                {
                    return CodeResultType.LineEnd;
                }
            }
            return null;
        }

        public override object Ended(Checker inElement)
        {
            if (!AllowInside && GetCurrentStack().Count > 1)
            {
                return null;
            }
            if (inElement.Start == "(")
            {
                return CodeResultType.InCurlyBracket;
            }
            return null;
        }

        public override object Started(Checker inElement)
        {
            if (inElement.Start == "(" && GetCurrentStack().Count == 1)
            {
                return CodeResultType.LineEnd;
            }
            return null;
        }
    }
}
