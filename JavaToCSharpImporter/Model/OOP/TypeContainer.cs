using JavaToCSharpConverter.Helper;
using JavaToCSharpConverter.Model.Splitter;
using System.Collections.Generic;
using System.Linq;

namespace JavaToCSharpConverter.Model
{
    /// <summary>
    /// Methodendefinition für eine Klasse
    /// </summary>
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
                var tmpExtends = inType.Substring(inType.IndexOf("extends "));

                tmpContainer.Extends = tmpExtends.Split(',').Select(inItem => inItem.Trim(' ')).ToList();
            }
            else if (inType.Contains("<"))
            {
                tmpContainer.Name = inType.Substring(0, inType.IndexOf("<"));
                var tmpInnerData = inType.Substring(inType.IndexOf("<") + 1, inType.Length - inType.IndexOf("<") - 2);
                var tmpSplitted = CodeSplitter.FileDataSplitter(tmpInnerData, new ClassInterfaceSplitter()).ToList();
                foreach (var tmpSplit in tmpSplitted)
                {
                    tmpContainer.GenericTypes.Add(tmpSplit.Item2);
                }
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
                return $"{Name}{(IsArray ? "[]" : "")}";
            }
            return $"{Name}<{string.Join(", ", GenericTypes)}>{(IsArray ? "[]" : "")}";
        }
    }
}
