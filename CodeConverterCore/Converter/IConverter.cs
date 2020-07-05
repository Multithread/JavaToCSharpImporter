using CodeConverterCore.Model;

namespace CodeConverterCore.Converter
{
    /// <summary>
    /// Interace for Convertion Data
    /// </summary>
    public interface IConverter
    {
        /// <summary>
        /// Create Comment from Comment-String
        /// </summary>
        /// <param name="inComment">Comment string (single or Multiline)</param>
        /// <param name="inDefinitionCommennt">Simple Comment, or Methode/Class definition Comment</param>
        string Comment(string inComment, bool inDefinitionCommennt = false);

        /// <summary>
        /// Modifier handling
        /// </summary>
        string Modifier(string inOldmodifier);
        
        /// <summary>
        /// Class Name handling
        /// </summary>
        string ClassName(ClassContainer inClass);

        /// <summary>
        /// MethodeParameter Handling
        /// </summary>
        string MethodeInParameter(FieldContainer inMethodeParameter);
    }
}
