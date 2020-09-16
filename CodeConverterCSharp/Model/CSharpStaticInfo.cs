using CodeConverterCore.Enum;
using System.Collections.Generic;

namespace CodeConverterCSharp.Model
{
    public static class CSharpStaticInfo
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
           {">> ", VariableOperatorType.BitShiftRightUnsigned},
           {"<< ", VariableOperatorType.BitShiftLeftUnsigned},

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

        public static Dictionary<VariableOperatorType, string> _operatorToString;//= new Dictionary<VariableOperatorType, string>

        public static string GetOperatorString(VariableOperatorType inOperator)
        {
            if (_operatorToString == null)
            {
                _operatorToString = new Dictionary<VariableOperatorType, string>();
                foreach (var tmpEntry in VariableOperators)
                {
                    _operatorToString.Add(tmpEntry.Value, tmpEntry.Key);
                }
            }
            return _operatorToString[inOperator];
        }
    }
}
