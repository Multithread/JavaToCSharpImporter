using static CodeConverterJava.Model.JavaParser;

namespace CodeConverterJava.Model
{
    /// <summary>
    /// FormalParamContext
    /// </summary>
    public  interface IFormalParameterContext
    {
         TypeTypeContext typeType();

        VariableDeclaratorIdContext variableDeclaratorId();

        VariableModifierContext[] variableModifier();

        VariableModifierContext variableModifier(int i);
    }
}
