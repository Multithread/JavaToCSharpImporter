using Antlr4.Runtime.Tree;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JavaToCSharpConverter.Helper
{
    public static class AntlrHelper
    {
        /// <summary>
        /// Get All Children of Tree Element
        /// </summary>
        /// <param name="inTree"></param>
        /// <returns></returns>
        public static IEnumerable<IParseTree> GetChildren(this IParseTree inTree)
        {
            if (inTree == null)
            {
                yield break;
            }
            for (var tmpI = 0; tmpI < inTree.ChildCount; tmpI++)
            {
                var tmpChild = inTree.GetChild(tmpI);
                yield return tmpChild;
            }
        }
    }
}
