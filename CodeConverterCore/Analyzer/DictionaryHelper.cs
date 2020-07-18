using System;
using System.Collections.Generic;
using System.Linq;

namespace CodeConverterCore.Analyzer
{
    public static class DictionaryHelper
    {
        /// <summary>
        /// Add Listable Value to Dictionary
        /// </summary>
        /// <typeparam name="TKey"></typeparam>
        /// <typeparam name="TValueEntry"></typeparam>
        /// <param name="inDictionary"></param>
        /// <param name="inKey"></param>
        /// <param name="inValue"></param>
        /// <returns>False if the Value was already added</returns>
        public static bool Add<TKey, TValueEntry>(this Dictionary<TKey, List<TValueEntry>> inDictionary, TKey inKey, TValueEntry inValue)
        {
            if (!inDictionary.TryGetValue(inKey, out var tmpList))
            {
                tmpList = new List<TValueEntry>();
                inDictionary.Add(inKey, tmpList);
            }
            if (tmpList.Contains(inValue))
            {
                return false;
            }
            tmpList.Add(inValue);
            return true;
        }
        /// <summary>
        /// Add Listable Value to Dictionary
        /// </summary>
        /// <typeparam name="TKey"></typeparam>
        /// <typeparam name="TValueEntry"></typeparam>
        /// <param name="inDictionary"></param>
        /// <param name="inKey"></param>
        /// <param name="inValue"></param>
        /// <returns>False if the Value was already added</returns>
        public static bool TryGetValue<TKey, TValueEntry>(this Dictionary<TKey, List<TValueEntry>> inDictionary, TKey inKey, Func<TValueEntry, bool> inListSearch, out TValueEntry outValue)
        {
            if (inDictionary.TryGetValue(inKey, out var tmpList))
            {
                var tmpVal = tmpList.FirstOrDefault(inListSearch);
                if (tmpVal != null)
                {
                    outValue = tmpVal;
                    return true;
                }
            }
            outValue = default(TValueEntry);
            return false;
        }
    }
}
