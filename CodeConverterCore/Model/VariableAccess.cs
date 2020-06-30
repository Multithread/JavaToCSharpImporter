using CodeConverterCore.Interface;

namespace CodeConverterCore.Model
{
    public class VariableAccess : ICodeEntry
    {
        /// <summary>
        /// Name of the Variable to access (filled from Code-Resolver)
        /// </summary>
        public ICodeEntry Access { get; set; }

        /// <summary>
        /// Next Child for Access 
        /// resolves what will be done further with the Access
        /// </summary>
        public ICodeEntry Child { get; set; }

        /// <summary>
        /// Base Code Entry with link to the actual source 
        /// (Field, Methode-call, Variable, inParameter) 
        /// filled during code-analysis
        /// </summary>
        public ICodeEntry BaseDataSource { get; set; }
    }
}
