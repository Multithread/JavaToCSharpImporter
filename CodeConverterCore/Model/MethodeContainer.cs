using Antlr4.Runtime.Tree;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Diagnostics;

namespace CodeConverterCore.Model
{
    /// <summary>
    /// Methodendefinition für eine Klasse
    /// </summary>
    [JsonObject(MemberSerialization.OptIn)]
    [DebuggerDisplay("{Name}")]
    public class MethodeContainer
    {
        public MethodeContainer() { }
        /// <summary>
        /// Name of the Methode
        /// </summary>
        [JsonProperty]
        public string Name
        {
            get => ParentContainer?.Name ?? _name;
            set => _name = value;
        }
        private string _name;

        /// <summary>
        /// Returntype. Might be void
        /// </summary>
        [JsonProperty]
        public TypeContainer ReturnType { get; set; }

        /// <summary>
        /// List of Generic Sub-Types
        /// </summary>
        [JsonProperty]
        public List<TypeContainer> GenericTypes { get; set; } = new List<TypeContainer>();

        /// <summary>
        /// A List of the Methode Params
        /// </summary>
        [JsonProperty]
        public List<FieldContainer> Parameter { get; set; } = new List<FieldContainer>();

        /// <summary>
        /// Attributes (private, public, abstract, override,...)
        /// </summary>
        [JsonProperty]
        public List<string> ModifierList { get; set; } = new List<string>();

        /// <summary>
        /// Constructor base calls (this and base)
        /// </summary>
        public MethodeCall ConstructorCall { get; set; }

        /// <summary>
        /// Methode Comment
        /// </summary>
        public string Comment { get; set; }

        /// <summary>
        /// Code, from Inside the Methode
        /// </summary>
        public CodeBlock Code { get; set; }

        /// <summary>
        /// Code, from Inside the Methode
        /// </summary>
        public IParseTree AntlrCode { get; set; }

        /// <summary>
        /// Filled, if the Methode is a Constructor with a Base Call (:this /:base)
        /// </summary>
        public MethodeContainer Constructorinfo { get; set; }

        /// <summary>
        /// Methode is a Property, if supported by the Language
        /// </summary>
        public bool IsProperty { get; set; }

        /// <summary>
        /// Parent Container when 
        /// </summary>
        public MethodeContainer ParentContainer { get; set; }

        /// <summary>
        /// Methode Parent if this Methode overrides another Methode
        /// </summary>
        public ClassContainer Parent { get; set; }

        public bool IsConstructor { get; set; }
    }
}
