using JavaToCSharpConverter.Interface;

namespace JavaToCSharpConverter.Model.OOP
{
    public class VariableDeclaration : ICodeEntry
    {
        /// <summary>
        /// Type of the Declaring Variable
        /// </summary>
        public TypeContainer Type { get; set; }

        /// <summary>
        /// Variablename
        /// </summary>
        public string Name { get; set; }
    }
}
