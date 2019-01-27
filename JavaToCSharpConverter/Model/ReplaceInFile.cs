using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace JavaToCSharpConverter.Model
{
    /// <summary>
    /// Suchen und ersetzen in Dateien
    /// </summary>
    public class ReplaceInFile : SearchInFile
    {
        public List<SearchAndReplacePattern> Replacelist { get; set; } = new List<SearchAndReplacePattern>();

        public override void Search()
        {
            var tmpFile = LoadFile();

            string tmpNewFile = tmpFile;
            foreach (var tmpReplace in Replacelist)
            {
                tmpNewFile = tmpReplace.Apply(tmpNewFile);
            }
            //Prüfem pb was geändert hat
            if (tmpNewFile != tmpFile)
            {
                System.IO.File.WriteAllText(FilePath, tmpNewFile);
                FoundCount = 1;
                ResultCache.Bag.Add(FilePath);
                Console.WriteLine("File Changed: " + FilePath);
            }
        }
    }
}
