using System.Collections.Generic;

namespace JavaToCSharpConverter.Model
{
    /// <summary>
    /// Variablendeklaration, Felddeklaration, Methodenparameter,...
    /// </summary>
    public class FieldContainer
    {
        /// <summary>
        /// Name des Feldes
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Name des Feldes
        /// </summary>
        public string Type { get; set; }

        /// <summary>
        /// Attributes (private, public, abstract, override,...)
        /// </summary>
        public List<string> ModifierList { get; set; } = new List<string>();

        /// <summary>
        /// Kommentar, welcher zu diesem Feld geschrieben wurde
        /// </summary>
        public string Comment { get; set; }

        /// <summary>
        /// Default Wert
        /// </summary>
        public string DefaultValue { get; set; }

        /// <summary>
        /// Existiert ein Default wert?
        /// </summary>
        public bool HasDefaultValue { get; set; }
    }
}
