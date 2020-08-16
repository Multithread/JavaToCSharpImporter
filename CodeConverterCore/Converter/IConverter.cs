using CodeConverterCore.Model;
using System.Collections.Generic;

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
        /// Mapp and Sort the Attributes from Java to C#
        /// </summary>
        /// <param name="inAttributeList"></param>
        /// <param name="inProperty"></param>
        /// <returns></returns>
        List<string> MapAndSortAttributes(List<string> inAttributeList, bool inProperty = false);

        /// <summary>
        /// Class Name handling
        /// </summary>
        string ClassName(ClassContainer inClass);

        /// <summary>
        /// MethodeParameter Handling
        /// </summary>
        string MethodeInParameter(FieldContainer inMethodeParameter);

        /// <summary>
        /// Namespace Renaming
        /// </summary>
        /// <param name="inNamespace"></param>
        /// <returns></returns>
        IEnumerable<string> Namespace(params string[] inNamespace);

        /// <summary>
        /// Change Methode Names to be matching C# names
        /// </summary>
        /// <param name="inMethode"></param>
        /// <returns>new Methode Name</returns>
        string MethodeName(MethodeContainer inMethode);

        /// <summary>
        /// Handle Things, not handled inside other code
        /// </summary>
        /// <param name="inClass"></param>
        void AnalyzerClassModifier(ClassContainer inClass);
    }
}
