using CodeConverterCore.Enum;
using System.Collections.Generic;

namespace CodeConverterJava.Model
{
    public static class JavaStaticInfo
    {
        public static Dictionary<string, VariableOperatorType> VariableOperators = new Dictionary<string, VariableOperatorType>
        {
           //Math Operators
           {"+", VariableOperatorType.Addition},
           {"-", VariableOperatorType.Substraction},
           {"*", VariableOperatorType.Multiplication},
           {"/", VariableOperatorType.Division},

           //Boolean Operators
           {"==", VariableOperatorType.Equals},
           {"!=", VariableOperatorType.NotEquals},
           {"&&", VariableOperatorType.And},
           {"||", VariableOperatorType.Or},
           {"<", VariableOperatorType.LessThan},
           {">", VariableOperatorType.MoreThan},
           {"<=", VariableOperatorType.LessOrEquals},
           {">=", VariableOperatorType.MoreOrEquals},
           {"^", VariableOperatorType.XOR},

           //Bitshift Operators
           {">>", VariableOperatorType.BitShiftRight},
           {"<<", VariableOperatorType.BitShiftLeft},
           {">>>", VariableOperatorType.BitShiftRightUnsigned},
           {"<<<", VariableOperatorType.BitShiftLeftUnsigned},
           {"&", VariableOperatorType.BitwiseAnd},
           {"|", VariableOperatorType.BitwiseOr},
           
           //VarOperations With Set
           {"++", VariableOperatorType.PlusPlus},
           {"--", VariableOperatorType.MinusMinus},
           {"+=", VariableOperatorType.PlusEquals},
           {"-=", VariableOperatorType.MinusEquals},
        };

        public static VariableOperatorType GetManipulator(string inOperatorAssString)
        {
            return VariableOperators[inOperatorAssString];
        }
    }
}
