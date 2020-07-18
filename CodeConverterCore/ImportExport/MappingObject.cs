namespace CodeConverterCore.ImportExport
{
    /// <summary>
    /// Alias Object
    /// </summary>
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
