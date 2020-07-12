using System.Diagnostics;

namespace CodeConverterCore.Model
{
    [DebuggerDisplay("{Namespace}.{Name}")]
    public class BaseType
    {
        public BaseType(string inName, string inNamespace)
        {
            Name = inName;
            Namespace = inNamespace;
        }

        /// <summary>
        /// Unknown Types or Types to be Defined
        /// </summary>
        /// <param name="inName"></param>
        public BaseType(string inName)
        {
            Name = inName;
            Namespace = "";
        }

        /// <summary>
        /// Name des Typs
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Typen Namespace
        /// </summary>
        public string Namespace { get; set; }

        /// <summary>
        /// Void BaseType
        /// </summary>
        public static BaseType Void = new BaseType("void");

        public override bool Equals(object obj)
        {
            return Equals(obj as BaseType);
        }

        public bool Equals(BaseType inComapreType)
        {
            if (Name != inComapreType.Name)
            {
                return false;
            }
            if (Namespace != inComapreType.Namespace)
            {
                return false;
            }
            return true;
        }
    }
}
