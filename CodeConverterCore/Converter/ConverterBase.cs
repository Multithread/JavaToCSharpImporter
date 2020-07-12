using CodeConverterCore.Model;

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
        public virtual string Comment(string inOldComment, bool inDefinitionCommennt = false)
        {
            return inOldComment;
        }

        /// <summary>
        /// Modifier handling
        /// </summary>
        public virtual string Modifier(string inOldmodifier)
        {
            return inOldmodifier;
        }

        /// <summary>
        /// Class Name handling
        /// </summary>
        public virtual string ClassName(ClassContainer inClass)
        {
            return inClass.Type.Type.Name;
        }

        /// <summary>
        /// Class Name handling
        /// </summary>
        public virtual string Namespace(string inNamespace)
        {
            return inNamespace;
        }

        /// <summary>
        /// MethodeParameter Handling
        /// </summary>
        public virtual string MethodeInParameter(FieldContainer inMethodeParameter)
        {
            return inMethodeParameter.Name;
        }
    }
}
