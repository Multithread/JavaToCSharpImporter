using CodeConverterCore.Interface;
using System;

namespace CodeConverterCore.Model
{
    public class ConstantValue:ICodeEntry
    {
        public object Value { get; set; }

        public Type Type { get; set; }

        /// <summary>
        /// Return Constant as Type with Value
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            if (Type == null) {
                return Value?.ToString();
            }
            return $"({Type.ToString()}){Value}";
        }
    }
}
