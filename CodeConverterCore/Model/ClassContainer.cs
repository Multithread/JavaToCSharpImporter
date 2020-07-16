using CodeConverterCore.Enum;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace CodeConverterCore.Model
{
    /// <summary>
    /// Definition einer Klasse
    /// </summary>
    [JsonObject(MemberSerialization.OptIn)]
    [DebuggerDisplay("{Namespace}: {Type}")]
    public class ClassContainer
    {
        /// <summary>
        /// Class-Name
        /// </summary>
        public string Name
        {
            get
            {
                return Type?.Name ?? "BBBBB";
            }
        }

        /// <summary>
        /// Class Type
        /// </summary>
        [JsonProperty]
        public TypeContainer Type { get; set; }

        /// <summary>
        /// Namespace der Klasse
        /// </summary>
        [JsonProperty]
        public string Namespace { get; set; }

        /// <summary>
        /// Comment in the Namespace of the Class
        /// </summary>
        public string NamespaceComment { get; set; }

        /// <summary>
        /// Class Comment
        /// </summary>
        public string Comment { get; set; }

        /// <summary>
        /// Usings, welche der Klasse angefügt sind.
        /// </summary>
        [JsonProperty]
        public List<string> UsingList { get; set; } = new List<string>();

        /// <summary>
        /// Parent and a List of Implemented Interfaces
        /// </summary>
        [JsonProperty]
        public List<TypeContainer> InterfaceList { get; set; } = new List<TypeContainer>();

        /// <summary>
        /// Klassenattribute (sealed, Abstract, ....)
        /// </summary>
        [JsonProperty]
        public List<string> ModifierList { get; set; } = new List<string>();

        /// <summary>
        /// Liste der Felder (Field, ggf. Property)
        /// </summary>
        [JsonProperty]
        public List<FieldContainer> FieldList { get; set; } = new List<FieldContainer>();

        /// <summary>
        /// Liste der Methoden
        /// </summary>
        [JsonProperty]
        public List<MethodeContainer> MethodeList { get; } = new List<MethodeContainer>();

        public void AddMethode(MethodeContainer inNewMethode)
        {
            MethodeList.Add(inNewMethode);
            inNewMethode.Parent = this;
        }

        /// <summary>
        /// List of inner Classes
        /// </summary>
        public List<ClassContainer> InnerClasses { get; set; } = new List<ClassContainer>();

        /// <summary>
        /// System Classes do not need to be put into the Output
        /// Loaded and Unknown Classes do net to be put into Output
        /// </summary>
        public ClassTypeEnum ClassType { get; set; } = ClassTypeEnum.Normal;

        /// <summary>
        /// CHeck for Empty
        /// </summary>
        /// <returns></returns>
        public bool IsEmpty()
        {
            if (string.IsNullOrEmpty(Name))
            {
                return true;
            }
            return false;
        }

        /// <summary>
        /// Has this Class been Corectly Converted into the IL language?
        /// </summary>
        public bool IsAnalyzed { get; internal set; }

        /// <summary>
        /// Ist it an Interface?
        /// </summary>
        /// <returns></returns>
        public bool IsInterface()
        {
            return ModifierList.Contains("interface");
        }

        public List<string> FullUsingList
        {
            get
            {
                return UsingList.Concat(new List<string> { Namespace }).ToList();
            }
        }
    }
}
