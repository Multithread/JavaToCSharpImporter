using Antlr4.Runtime.Tree;
using System.Collections.Generic;

namespace JavaToCSharpConverter.Model
{
    /// <summary>
    /// Methodendefinition für eine Klasse
    /// </summary>
    public class MethodeContainer
    {
        /// <summary>
        /// Name of the Methode
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Returntype. Might be void
        /// </summary>
        public string ReturnType { get; set; }

        /// <summary>
        /// A List of the Methode Params
        /// </summary>
        public List<FieldContainer> Parameter { get; set; } = new List<FieldContainer>();

        /// <summary>
        /// Attributes (private, public, abstract, override,...)
        /// </summary>
        public List<string> ModifierList { get; set; } = new List<string>();

        /// <summary>
        /// Methode Comment
        /// </summary>
        public string Comment { get; set; }

        /// <summary>
        /// Code, from Inside the Methode
        /// </summary>
        public string Code { get; set; }

        /// <summary>
        /// Code, from Inside the Methode
        /// </summary>
        public IParseTree AntlrCode { get; set; }

        /// <summary>
        /// Filled, if the Methode is a Constructor with a Base Call (:this /:base)
        /// </summary>
        public MethodeContainer Constructorinfo { get; set; }
    }
}
