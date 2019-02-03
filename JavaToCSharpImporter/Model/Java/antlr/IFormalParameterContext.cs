using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static JavaToCSharpConverter.Model.Java.JavaParser;

namespace JavaToCSharpConverter.Model.Java
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
