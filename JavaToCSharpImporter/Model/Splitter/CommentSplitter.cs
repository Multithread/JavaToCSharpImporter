using JavaToCSharpConverter.Helper;
using JavaToCSharpConverter.Interface;

namespace JavaToCSharpConverter.Model.Splitter
{
    public class CommentSplitter : CodeSplitterEvents
    {
        public bool AllowInside { get; set; } = false;
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
            if (inElement.Start == "//" || inElement.Start == "/*")
            {
                return CodeResultType.Comment;
            }
            if (inElement.Start == "{")
            {
                return CodeResultType.InCurlyBracket;
            }
            return null;
        }

        public override object Started(Checker inElement)
        {
            if (inElement.Start == "{" && GetCurrentStack().Count == 1)
            {
                return CodeResultType.None;
            }
            return null;
        }
    }
}
