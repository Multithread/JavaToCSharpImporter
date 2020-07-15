using CodeConverterCore.Helper;

namespace JavaToCSharpConverter
{
    public class Program
    {

        static void Main(string[] args)
        {
            var tmpJavaSourcePath = @"C:\Data\LucenTestData\Working\";
            var tmpCSharpOutputpath = @"C:\Data\LucenTestData\Result\";

            ConverterHelper.ConvertFiles(tmpJavaSourcePath, tmpCSharpOutputpath);
        }

    }
}
