using JavaToCSharpConverter.Helper;
using JavaToCSharpConverter.Interface;
using System.Linq;

namespace JavaToCSharpConverter.Model.Splitter
{
    /// <summary>
    /// Split the Code lines into smaller Chunks
    /// </summary>
    public class CodeFormatSplitter : CodeSplitterEvents
    {
        public override object CurrentCharacter(char inChar)
        {
            if (inChar == ';')
            {
                var tmpStack = GetCurrentStack();
                if (tmpStack.Count == 0 || GetCurrentStack().First().Start != "(")
                {
                    return FormatResultType.EndOfLine;
                }
            }
            return null;
        }

        public override object Ended(Checker inElement)
        {
            if (inElement.Start == "{")
            {
                return FormatResultType.CurlyEnd;
            }
            if (inElement.Start == "//")
            {
                return FormatResultType.SingleLineCommentEnd;
            }
            if (inElement.Start == "/*")
            {
                return FormatResultType.MultiLineCommentEnd;
            }
            return null;
        }

        public override object Started(Checker inElement)
        {
            if (inElement.Start == "{")
            {
                return FormatResultType.CurlyStart;
            }
            if (inElement.Start == "//")
            {
                return FormatResultType.SingleLineCommentStart;
            }
            if (inElement.Start == "/*")
            {
                return FormatResultType.MultiLineCommentStart;
            }
            return null;
        }
    }
}
