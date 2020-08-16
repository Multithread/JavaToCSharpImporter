using JavaToCSharpConverter.Model;
using System.Collections.Generic;

namespace CodeConverterCSharp.Lucenene
{
    public class ConverterLucene : ConverterJavaToCSharp
    {

        /// <summary>
        /// Namespace Changes to fit C# Namings
        /// </summary>
        /// <param name="inNamespace"></param>
        /// <returns></returns>
        public override IEnumerable<string> Namespace(params string[] inNamespace)
        {
            foreach (var tmpNamespace in base.Namespace(inNamespace))
            {
                if (tmpNamespace.StartsWith("org.apache.lucene"))
                {
                    yield return tmpNamespace.Replace("org.apache.lucene", "LuceNET");
                }
                else if (tmpNamespace == "java.lang")
                {
                    yield return "System";
                }
                else
                {
                    yield return tmpNamespace;
                }
            }
        }
    }
}
