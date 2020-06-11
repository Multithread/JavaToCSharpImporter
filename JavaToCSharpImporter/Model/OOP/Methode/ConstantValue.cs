using JavaToCSharpConverter.Interface;
using System;

namespace JavaToCSharpConverter.Model.OOP
{
    public class ConstantValue:ICodeEntry
    {
        public object Value { get; set; }

        public Type Type { get; set; }
    }
}
