namespace JavaToCSharpConverter.Interface
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
}
