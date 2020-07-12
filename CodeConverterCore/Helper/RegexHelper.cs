using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace CodeConverterCore.Helper
{
    public partial class RegexHelper
    {
        public static Regex WordCheck = new Regex("\\w", RegexOptions.Compiled);

        public static Regex NumberCheck = new Regex(@"^-?[0-9][0-9,\.]+(d|m|f|L|UL|U){0,1}$", RegexOptions.Compiled);

        public static Regex NameStartsWith_In = new Regex("^(i|I)n([A-Z0-9]){1,1}", RegexOptions.Compiled);

        public static Regex NameStartsWith_Out = new Regex("^(o|O)ut([A-Z0-9]){1,1}", RegexOptions.Compiled);

        public static Regex NameStartsWith_Tmp = new Regex("^(t|T)mp([A-Z0-9]){1,1}", RegexOptions.Compiled);

        /// <summary>
        /// Check if the FIrst char in the String is an Upper char letter
        /// </summary>
        /// <param name="inInput">inputstring</param>
        /// <returns></returns>
        public static bool IsFirstCharUpper(string inInput)
        {
            if (string.IsNullOrEmpty(inInput))
            {
                return false;
            }
            if (inInput[0] > 64 && inInput[0] < 91)
            {
                return true;
            }
            return false;
        }
    }
}
