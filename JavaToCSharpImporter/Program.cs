using JavaToCSharpConverter.Helper;

namespace JavaToCSharpConverter
{
    public class Program
    {

        static void Main(string[] args)
        {
            var tmpJavaSourcePath = @"C:\Data\LucenTestData\";
            var tmpCSharpOutputpath = @"Z:\Result\Code\";

            ConverterHelper.ConvertFiles(tmpJavaSourcePath, tmpCSharpOutputpath);
        }

    }
}
