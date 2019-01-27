using IniParser;
using IniParser.Model;
using System.Collections.Generic;

namespace JavaToCSharpConverter.Helper
{
    public static class DataHelper
    {
        public static List<string> LoadJavaBaseFiles()
        {
            return new List<string>();
        }

        /// <summary>
        /// Load an Ini File
        /// </summary>
        /// <param name="inPath"></param>
        /// <returns></returns>
        public static IniData LoadIni(string inPath)
        {
            var parser = new FileIniDataParser();
            IniData data = parser.ReadFile(inPath);

            return data;
        }
    }
}
