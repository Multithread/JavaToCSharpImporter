using JavaToCSharpConverter.Model.OOP.Enum;
using System.Collections.Generic;

namespace JavaToCSharpConverter.Model.Java
{
    public static class JavaStaticInfo
    {
        public static Dictionary<string, VariableManipulatorType> MathElements = new Dictionary<string, VariableManipulatorType>
        {
           {"+", VariableManipulatorType.Addition},
           {"-", VariableManipulatorType.Substraction},
           {"*", VariableManipulatorType.Multiplication},
           {"/", VariableManipulatorType.Division},
           {"^", VariableManipulatorType.PowerOf},
        };

        public static Dictionary<string, VariableManipulatorType> BooleanOperators = new Dictionary<string, VariableManipulatorType>
        {
           {"==", VariableManipulatorType.Addition},
           {"!=", VariableManipulatorType.Substraction},
           {"&&", VariableManipulatorType.Substraction},
           {"||", VariableManipulatorType.Substraction},
           {"|", VariableManipulatorType.Substraction},
           {"&", VariableManipulatorType.Substraction},
        };

        public static VariableManipulatorType GetManipulator(string inOperatorAssString)
        {
            return MathElements[inOperatorAssString];
        }
    }
}
