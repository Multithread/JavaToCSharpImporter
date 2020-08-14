using JavaToCSharpConverter.Model;

namespace CodeConverterCSharp.Lucenene
{
    public class ConverterLucene : ConverterJavaToCSharp
    {

        /// <summary>
        /// Namespace Changes to fit C# Namings
        /// </summary>
        /// <param name="inNamespace"></param>
        /// <returns></returns>
        public override string Namespace(string inNamespace)
        {
            if (inNamespace.StartsWith("org.apache.lucene"))
            {
                inNamespace = inNamespace.Replace("org.apache.lucene", "LuceNET");
            }
            if (inNamespace == "java.lang")
            {
                return "System";
            }
            return base.Namespace(inNamespace);
        }
    }
}
