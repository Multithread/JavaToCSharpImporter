using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace JavaToCSharpConverter.Model
{
    /// <summary>
    /// Definition einer Klasse
    /// </summary>
    [DebuggerDisplay("{Namespace}: {Name}")]
    public class ClassContainer
    {
        /// <summary>
        /// Class-Name
        /// </summary>
        public string Name
        {
            get
            {
                return Type.Name;
            }
        }

        /// <summary>
        /// Class Type
        /// </summary>
        public TypeContainer Type { get; set; }

        /// <summary>
        /// Namespace der Klasse
        /// </summary>
        public string Namespace { get; set; }

        /// <summary>
        /// Class Comment
        /// </summary>
        public string Comment { get; set; }

        /// <summary>
        /// Usings, welche der Klasse angefügt sind.
        /// </summary>
        public List<string> UsingList { get; set; } = new List<string>();

        /// <summary>
        /// List of Generic Type params (Class<T>, Generic<B,B2>) to be recognized later
        /// </summary>
        public List<string> GenericTypeParamList { get; set; } = new List<string>();

        /// <summary>
        /// Parent and a List of Implemented Interfaces
        /// </summary>
        public List<string> InterfaceList { get; set; } = new List<string>();

        /// <summary>
        /// Klassenattribute (sealed, Abstract, ....)
        /// </summary>
        public List<string> AttributeList { get; set; } = new List<string>();

        /// <summary>
        /// Liste der Felder (Field, ggf. Property)
        /// </summary>
        public List<FieldContainer> FieldList { get; set; } = new List<FieldContainer>();

        /// <summary>
        /// Liste der Methoden
        /// </summary>
        public List<MethodeContainer> MethodeList { get; set; } = new List<MethodeContainer>();

        /// <summary>
        /// List of inner Classes
        /// </summary>
        public List<ClassContainer> InnerClasses { get; set; } = new List<ClassContainer>();

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
        /// Ist it an Interface?
        /// </summary>
        /// <returns></returns>
        public bool IsInterface()
        {
            return AttributeList.Contains("interface");
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
