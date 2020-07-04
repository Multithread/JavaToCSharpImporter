namespace CodeConverterCore.Converter
{
    /// <summary>
    /// Helper to Convert Data
    /// </summary>
    public class ConverterBase : IConverter
    {
        /// <summary>
        /// Create Comment from Comment-String
        /// </summary>
        /// <param name="inComment">Comment string (single or Multiline)</param>
        /// <param name="inDefinitionCommennt">Simple Comment, or Methode/Class definition Comment</param>
        public virtual string Comment(string inOldComment,bool inDefinitionCommennt = false)
        {
            return inOldComment;
        }
    }
}
