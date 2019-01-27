using JavaToCSharpConverter.Helper;
using JavaToCSharpConverter.Interface;

namespace JavaToCSharpConverter.Model.Splitter
{
    public class FullFlatSplitter : CommentSplitter
    {
        public override object CurrentCharacter(char inChar)
        {
            if (inChar == ';')
            {
                if (GetCurrentStack().Count == 0)
                {
                    return CodeResultType.LineEnd;
                }
            }
            return null;
        }
    }
}
