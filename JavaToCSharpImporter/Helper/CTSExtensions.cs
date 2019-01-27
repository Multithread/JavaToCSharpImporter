using JavaToCSharpConverter.Model;
using System;
using System.Collections.Generic;
using System.Linq;

namespace JavaToCSharpConverter.Helper
{
    public static class CTSExtensions
    {
        /// <summary>
        /// Forach auf einer Liste ausführen
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="inData"></param>
        /// <param name="inAction"></param>
        /// <returns></returns>
        public static IEnumerable<T> Foreach<T>(this IEnumerable<T> inData, Action<T> inAction)
        {
            foreach (var tmpData in inData)
            {
                inAction(tmpData);
                yield return tmpData;
            }
        }

        /// <summary>
        /// Uppens the first Char
        /// </summary>
        /// <param name="inMethodeName"></param>
        /// <returns></returns>
        public static string PascalCase(this string inMethodeName)
        {
            if (inMethodeName.Length < 2)
            {
                return inMethodeName.ToUpper();
            }
            return inMethodeName.First().ToString().ToUpper() + inMethodeName.Substring(1);
        }

        /// <summary>
        /// Uppens the first Char
        /// </summary>
        /// <param name="inMethodeName"></param>
        /// <returns></returns>
        public static int FirstIndexofAny(this string inText, char[] inIndexSearch)
        {
            var tmpFirstIndex = inText.Length + 1;
            foreach (var tmpChar in inIndexSearch)
            {
                var tmpIndexOf = inText.IndexOf(tmpChar);
                if (tmpIndexOf != -1)
                {
                    tmpFirstIndex = Math.Min(tmpFirstIndex, tmpIndexOf);
                }
            }

            if (tmpFirstIndex == inText.Length + 1)
            {
                return -1;
            }
            return tmpFirstIndex;
        }

        /// <summary>
        /// Gets all Valid Types for a Generic Value.
        /// The Last one might be 'IsArray'
        /// </summary>
        /// <param name="inFullType"></param>
        /// <returns></returns>
        public static List<string> GetGenericObjectsForType(this string inFullType)
        {
            return inFullType
                .Split(new char[] { '<', '>', ',' })
                .Select(inItem => inItem.Trim(' '))
                .Where(inItem => !string.IsNullOrEmpty(inItem))
                .Skip(1) //Skip the first Element (Class Name)
                .ToList();
        }
    }
}
