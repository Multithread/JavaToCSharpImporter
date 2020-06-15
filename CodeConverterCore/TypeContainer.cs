using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace JavaToCSharpConverter.Model
{
    /// <summary>
    /// Methodendefinition für eine Klasse
    /// </summary>
    [DebuggerDisplay("{ToString()}")]
    public class TypeContainer
    {
        /// <summary>
        /// Typename (mostly another Class)
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// List of Generic Sub-Types
        /// </summary>
        public List<TypeContainer> GenericTypes { get; set; } = new List<TypeContainer>();

        /// <summary>
        /// Extends from Generic Types
        /// </summary>
        public List<string> Extends { get; set; } = new List<string>();

        /// <summary>
        /// Is it an Array
        /// </summary>
        public bool IsArray { get; set; }

        /// <summary>
        /// Convert String inTo usefull Type
        /// </summary>
        /// <param name="inType"></param>
        public static implicit operator TypeContainer(string inType)
        {
            var tmpContainer = new TypeContainer();
            if (inType.EndsWith("[]"))
            {
                tmpContainer.IsArray = true;
                inType = inType.Substring(0, inType.Length - 2);
            }
            if (inType.Contains("extends"))
            {
                tmpContainer.Name = inType.Split(' ')[0];
                var tmpExtends = inType.Substring(inType.IndexOf("extends"));

                tmpContainer.Extends = tmpExtends.Split(',').Select(inItem => inItem.Trim(' ')).ToList();
            }
            else if (inType.Contains("<"))
            {
                tmpContainer.Name = inType.Substring(0, inType.IndexOf("<"));
                var tmpInnerData = inType.Substring(inType.IndexOf("<") + 1, inType.Length - inType.IndexOf("<") - 2);
                throw new Exception("Nope");
            }
            else
            {
                tmpContainer.Name = inType;
            }

            return tmpContainer;
        }

        /// <summary>
        /// Override ToString
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            if (GenericTypes.Count == 0)
            {
                if (Extends.Count > 0)
                {
                    return $"{Name} extends {string.Join(", ", Extends)}{(IsArray ? "[]" : "")}";
                }
                return $"{Name}{(IsArray ? "[]" : "")}";
            }
            if (Extends.Count > 0)
            {
                return $"{Name}<{string.Join(", ", GenericTypes)} extends {string.Join(", ", Extends)}>{(IsArray ? "[]" : "")}";
            }
            return $"{Name}<{string.Join(", ", GenericTypes)}>{(IsArray ? "[]" : "")}";
        }

        /// <summary>
        /// OVerride Equals to be correct for our usage
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public override bool Equals(object obj)
        {
            var tmpOther = obj as TypeContainer;
            if (tmpOther.Name != Name)
            {
                return false;
            }
            if (tmpOther.GenericTypes.Count != GenericTypes.Count)
            {
                return false;
            }
            for (var tmpI = 0; tmpI < GenericTypes.Count; tmpI++)
            {
                if (tmpOther.GenericTypes[tmpI] != GenericTypes[tmpI])
                {
                    return false;
                }
            }

            if (tmpOther.Extends.Count != Extends.Count)
            {
                return false;
            }
            for (var tmpI = 0; tmpI < Extends.Count; tmpI++)
            {
                if (tmpOther.Extends[tmpI] != Extends[tmpI])
                {
                    return false;
                }
            }
            return true;
        }
    }
}
