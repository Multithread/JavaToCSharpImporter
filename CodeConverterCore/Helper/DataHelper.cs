using IniParser;
using IniParser.Model;
using System.Collections.Generic;
using System.IO;

namespace CodeConverterCore.Helper
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
        public static IniData LoadIniByPath(string inPath)
        {
            var parser = new FileIniDataParser();
            IniData data = parser.ReadFile(inPath);

            return data;
        }
        /// <summary>
        /// Load an Ini File
        /// </summary>
        /// <param name="inPath"></param>
        /// <returns></returns>
        public static IniData LoadIni(string inIniAsText)
        {
            var parser = new FileIniDataParser();
            var stream = new MemoryStream();
            var writer = new StreamWriter(stream);
            writer.Write(inIniAsText);
            writer.Flush();
            stream.Position = 0;
            
            IniData data = parser.ReadData(new System.IO.StreamReader(stream));

            return data;
        }
    }
}
