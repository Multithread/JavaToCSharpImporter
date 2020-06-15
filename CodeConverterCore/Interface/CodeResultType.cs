namespace CodeConverterCore.Interface
{
    public enum CodeResultType
    {
        None = 1,
        Comment = 2,
        InCurlyBracket = 3,
        LineEnd = 4,
        InBracket = 5,
    }

    public enum CodeLineResultType
    {
        EndOfLine = 1,
        Comment = 2,
        CurlyStart = 3,
        CurlyEnd = 4,
    }


    public enum FormatResultType
    {
        EndOfLine = 1,
        CurlyStart = 2,
        CurlyEnd = 3,
        SingleLineCommentStart = 4,
        SingleLineCommentEnd = 5,
        MultiLineCommentStart = 6,
        MultiLineCommentEnd = 7,
    }
}
