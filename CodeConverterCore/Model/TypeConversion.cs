using CodeConverterCore.Interface;

namespace CodeConverterCore.Model
{
    public class TypeConversion : ICodeEntry
    {
        public TypeConversion() { }

        /// <summary>
        /// Type to be Converted to
        /// </summary>
        public TypeContainer Type { get; set; }

        /// <summary>
        /// Value on the right side that needs to be Converted
        /// </summary>
        public CodeBlock PreconversionValue { get; set; }

        /// <summary>
        /// Ist the Conversion "as" or fix.
        /// </summary>
        /// <value>
        /// false: (String)inObject
        /// true: inObject as String
        /// </value>
        public bool IsAsConversion { get; set; }

        /// <summary>
        /// Return Constant as Type with Value
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            if (Type == null)
            {
                return PreconversionValue?.ToString();
            }
            return $"({Type}){PreconversionValue}";
        }
    }
}
