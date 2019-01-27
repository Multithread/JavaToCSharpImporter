using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace JavaToCSharpConverter.Model
{
    public class SearchAndReplacePattern
    {
        public string SearchPattern { get; set; }
        public RegexOptions Options { get; set; }
        public string ReplacePattern { get; set; }
        public int FoundCount { get; set; }

        public string Apply(string inText)
        {
            var tmpRegex = new Regex(SearchPattern, Options);
            if (string.IsNullOrEmpty(ReplacePattern))
            {
                //Search
                FoundCount = tmpRegex.Matches(inText).Count;
                if (FoundCount > 0)
                {
                    Console.WriteLine("Found: " + FoundCount);
                }
                return inText;
            }

            //Replace
            var tmpNewText = tmpRegex.Replace(inText, ReplacePattern);
            if (tmpNewText != inText)
            {
                FoundCount = 1;
            }
            return tmpNewText;
        }
    }
}
