using System.Collections.Generic;

namespace JavaToCSharpConverter.Model.OOP
{
    public class MissingFunctionInformation
    {
        /// <summary>
        /// Methode Name
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Return Type
        /// </summary>
        public TypeContainer ReturnType { get; set; }

        /// <summary>
        /// List of the Methode Params
        /// </summary>
        public List<FunctionParam> ParamList { get; set; } = new List<FunctionParam>();
    }

    public class FunctionParam
    {
        /// <summary>
        /// Name
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Type
        /// </summary>
        public TypeContainer Type { get; set; }

        /// <summary>
        /// Methode Param Modifier
        /// </summary>
        public List<string> ModifierList { get; set; } = new List<string>();
    }
}
