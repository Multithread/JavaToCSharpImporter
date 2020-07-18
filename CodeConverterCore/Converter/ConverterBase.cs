using CodeConverterCore.Model;
using System.Collections.Generic;

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

        /// <summary>
        /// Map and Sort Attributes of classes, fields and methods
        /// </summary>
        /// <param name="inAttributeList"></param>
        /// <param name="inProperty"></param>
        /// <returns></returns>
        public virtual List<string> MapAndSortAttributes(List<string> inAttributeList, bool inProperty = false)
        {
            return inAttributeList;
        }

        /// <summary>
        /// Change Methode Names to be matching C# names
        /// </summary>
        /// <param name="inMethode"></param>
        /// <returns></returns>
        public virtual string MethodeName(MethodeContainer inMethode)
        {
            return inMethode.Name;
        }
    }
}
