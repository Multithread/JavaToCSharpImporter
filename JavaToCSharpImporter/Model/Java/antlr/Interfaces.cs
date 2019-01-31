using Antlr4.Runtime.Tree;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JavaToCSharpConverter.Model
{
   public interface IClassAndInterfaceBaseData
    {
        ITerminalNode IDENTIFIER();

        ITerminalNode EXTENDS();
    }
}
