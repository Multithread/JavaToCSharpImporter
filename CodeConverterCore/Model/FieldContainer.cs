using System.Collections.Generic;
using System.Diagnostics;

namespace CodeConverterCore.Model
{
    /// <summary>
    /// Felddeklaration, Methodenparameter,...
    /// Every Field Container is also a Variable Declarion for Code Usage
    /// </summary>
    [DebuggerDisplay("{Type}: {Name}")]
    public class FieldContainer: VariableDeclaration
    {
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
        public CodeBlock DefaultValue { get; set; }
    }
}
