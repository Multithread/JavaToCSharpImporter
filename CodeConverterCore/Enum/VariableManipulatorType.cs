namespace CodeConverterCore.Enum
{
    public enum VariableOperatorType
    {
        //Matzh Operators
        Addition=1,
        Substraction=2,
        Multiplication=3,
        Division = 4,

        //Boolean operators
        Equals = 10,
        NotEquals = 11,
        MoreThan = 12,
        MoreOrEquals = 13,
        LessThan = 14,
        LessOrEquals = 15,
        And = 16,
        Or= 17,
        XOR = 18,

        //Bitshift Operators
        BitShiftRight = 20,
        BitShiftLeft = 21,
        BitShiftRightUnsigned = 22,
        BitShiftLeftUnsigned = 23,
        BitwiseAnd = 24,
        BitwiseOr = 25,

        //MathEqualsOperatiosn
        PlusPlus = 31,
        MinusMinus = 32,
        PlusEquals = 33,
        MinusEquals = 34,
    }
}
