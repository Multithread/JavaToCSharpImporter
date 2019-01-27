using JavaToCSharpConverter.Model;
using System.Collections.Generic;

namespace JavaToCSharpConverter.Interface
{
    public interface INameConverter
    {
        /// <summary>
        /// Change the Namespace
        /// </summary>
        string ChangeNamespace(string inNamespace);

        /// <summary>
        /// Change the Name of a Methode
        /// Consistency Required for the Code to work Propertly
        /// </summary>
        string ChangeMethodeName(string inMethodeName);

        /// <summary>
        /// Change the Name of a Methode
        /// Consistency Required for the Code to work Propertly
        /// </summary>
        List<string> MapAndSortAttributes(List<string> inAttributeList, bool inProperty = false);

        /// <summary>
        /// Return the Class for a spezific Type
        /// </summary>
        /// <param name="inType"></param>
        /// <param name="inNamespaces"></param>
        /// <returns></returns>
        ClassContainer GetClassForType(string inType, List<string> inNamespaces);

        /// <summary>
        /// Map a Type from the Old langugae to the New One
        /// </summary>
        /// <returns>Full Qualified Type</returns>
        string MapType(string inType, List<string> inNamespaces);

        /// <summary>
        /// Map a Function from the Old langugae to the New One
        /// </summary>
        /// <returns>New Name of the Methode</returns>
        string MapFunction(string inFunction, string inType, List<string> inNamespaces);
    }
}
