using System.Diagnostics;

namespace CodeConverterCore.ImportExport
{
    /// <summary>
    /// Alias Object
    /// </summary>
    [DebuggerDisplay("{Namespace} {ClassName} {MethodeName}")]
    public class MappingObject
    {
        /// <summary>
        /// Class namespace
        /// </summary>
        public string Namespace { get; set; }

        /// <summary>
        /// Name of the class used for this mapping
        /// </summary>
        public string ClassName { get; set; }

        /// <summary>
        /// Name of the Methode used for this mapping, can be null
        /// </summary>
        public string MethodeName { get; set; }

        /// <summary>
        /// Write the Methode as Property if supported by the output Language
        /// </summary>
        public bool MethodeAsProperty { get; set; }

        public bool IsMethodeMapping()
        {
            return !string.IsNullOrEmpty(MethodeName);
        }

        public bool IsClassMapping()
        {
            return string.IsNullOrEmpty(MethodeName);
        }
    }
}
