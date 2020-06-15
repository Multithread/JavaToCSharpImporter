using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace JavaToCSharpConverter.Model
{
    /// <summary>
    /// Anzahl Element in einer Datei ausfindig machen
    /// </summary>
    public class SearchInFile
    {
        public string FilePath { get; set; }

        public string SearchPattern { get; set; }

        protected string LoadFile()
        {
            return System.IO.File.ReadAllText(FilePath);
        }

        public virtual void Search()
        {
            var tmpFile = LoadFile();
            var tmpRegex = new Regex(SearchPattern);
            FoundCount = tmpRegex.Matches(tmpFile).Count;
        }

        public int FoundCount { get; set; }
    }
}
