using JavaToCSharpConverter.Helper;
using JavaToCSharpConverter.Interface;

namespace JavaToCSharpConverter.Model.Splitter
{
    /// <summary>
    /// Split the Code lines into smaller Chunks
    /// </summary>
    public class CodeLineSplitter : CodeSplitterEvents
    {
        public override object CurrentCharacter(char inChar)
        {
            if (inChar == ';')
            {
                if (GetCurrentStack().Count == 0)
                {
                    return CodeLineResultType.EndOfLine;
                }
            }
            return null;
        }

        public override object Ended(Checker inElement)
        {
            if (GetCurrentStack().Count > 1)
            {
                return null;
            }
            if (inElement.Start == "//" || inElement.Start == "/*")
            {
                return CodeLineResultType.Comment;
            }
            if (inElement.Start == "{")
            {
                return CodeLineResultType.CurlyEnd;
            }
            return null;
        }

        public override object Started(Checker inElement)
        {
            if (inElement.Start == "{" && GetCurrentStack().Count == 1)
            {
                return CodeLineResultType.CurlyStart;
            }
            return null;
        }
    }
}
