using JavaToCSharpConverter.Interface;
using System.Collections.Generic;

namespace JavaToCSharpConverter.Model.OOP
{
    /// <summary>
    /// Current State of the Code
    /// </summary>
    public class CodeState
    {
        public CodeState(INameConverter inConverter, List<string> inUsingList)
        {
            _converter = inConverter;
            _usingList = inUsingList;
        }
        private INameConverter _converter;
        private List<string> _usingList;

        private int LeftSpacing = 1;

        public void AddSpacing()
        {
            LeftSpacing++;
        }
        public void ReduceSpacing()
        {
            LeftSpacing--;
        }

        public string GetSpacing()
        {
            var tmpString = "";

            for (var tmpI = 0; tmpI < LeftSpacing; tmpI++)
            {
                tmpString += "    ";
            }
            return tmpString;
        }

        //Add a Variable to the Code State
        public void AddVariable(string inType, string inName, bool inForceAdd = true)
        {
            var tmpClass = _converter.GetClassForType(inType, _usingList);
            if (tmpClass == null)
            {
                //throw new Exception("not found");
            }
            if (!inForceAdd)
            {
                if (_typeDictionary.ContainsKey(inName))
                {
                    return;
                }
            }
            _typeDictionary.Add(inName, tmpClass);
        }

        /// <summary>
        /// Clear all Variables
        /// </summary>
        public void ClearVariableList()
        {
            _MethodeParams.Clear();
            _typeDictionary.Clear();
        }

        /// <summary>
        /// Add a Variable to the Code State
        /// </summary>
        /// <param name="inType"></param>
        /// <param name="inName"></param>
        /// <param name="inForceAdd"></param>
        public void AddMethodeParam(string inType, string inName, bool inForceAdd = true)
        {
            AddVariable(inType, inName, inForceAdd);
            _MethodeParams.Add(inName);
        }

        //Add a Variable to the Code State
        public bool HasVariable(string inName)
        {
            return _typeDictionary.ContainsKey(inName);
        }

        public TypeContainer CurrentType { get; set; }

        /// <summary>
        /// Is it a Methode in Params?
        /// </summary>
        /// <param name="inName"></param>
        /// <returns></returns>
        public bool IsVariableMethodeParam(string inName)
        {
            return _MethodeParams.Contains(inName);
        }

        /// <summary>
        /// Typ aufgrund des Namens laden
        /// </summary>
        /// <param name="inName"></param>
        /// <returns></returns>
        public ClassContainer GetType(string inName)
        {
            if (_typeDictionary.TryGetValue(inName, out var tmpClass))
            {
                return tmpClass;
            }
            return null;
        }

        private Dictionary<string, ClassContainer> _typeDictionary = new Dictionary<string, ClassContainer>();
        private HashSet<string> _MethodeParams = new HashSet<string>();
    }
}
