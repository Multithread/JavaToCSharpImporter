using Antlr4.Runtime.Tree;
using static CodeConverterJava.Model.JavaParser;

namespace CodeConverterCore.Model
{
    public interface IClassAndInterfaceBaseData
    {
        ITerminalNode IDENTIFIER();

        ITerminalNode EXTENDS();

        TypeParametersContext typeParameters();

        TypeListContext typeList();
    }
}
