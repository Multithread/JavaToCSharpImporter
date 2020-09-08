namespace CodeConverterCore.ImportExport
{
    /// <summary>
    /// Definition to replace a string inside a text
    /// </summary>
    public class StringReplacement
    {
        /// <summary>
        /// 
        /// </summary>
        public string NameRegex { get; set; }

        /// <summary>
        /// Text to look for
        /// </summary>
        public string SourceText { get; set; }

        /// <summary>
        /// Replacement text
        /// </summary>
        public string ReplacementText { get; set; }

        /// <summary>
        /// Contains a Using, if one is required after this replacement
        /// </summary>
        public string RequiredUsing { get; set; }
    }
}
