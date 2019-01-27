using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JavaToCSharpConverter.Helper
{
    public static class StringHelper
    {

        public static string RemoveNewlines(this string inData)
        {
            return inData.Replace("\n", "").Replace("\r", "");
        }
    }
}
