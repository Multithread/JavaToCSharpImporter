using JavaToCSharpConverter.Interface;
using System;
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
    }
}
