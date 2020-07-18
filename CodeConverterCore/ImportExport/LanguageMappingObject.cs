namespace CodeConverterCore.ImportExport
{
    /// <summary>
    /// Alias Object
    /// </summary>
    public class LanguageMappingObject
    {
        /// <summary>
        /// Source information about the system class (language the code has been loaded in)
        /// </summary>
        public MappingObject Source { get; set; }

        /// <summary>
        /// Target information about the system class (language the code shall be written in)
        /// </summary>
        public MappingObject Target { get; set; }
    }
}
