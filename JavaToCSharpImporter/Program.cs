using CodeConverterCore.Helper;
using System;
using System.Diagnostics;

namespace JavaToCSharpConverter
{
    public class Program
    {

        static void Main(string[] args)
        {
            var tmpJavaSourcePath = @"C:\Data\LucenTestData\Working\";
            var tmpCSharpOutputpath = @"C:\Data\LucenTestData\Result\";

            var tmpTimer = Stopwatch.StartNew();
            ConverterHelper.ConvertFiles(tmpJavaSourcePath, tmpCSharpOutputpath);

            tmpTimer.Stop();
            Console.WriteLine("");
            Console.WriteLine("Miliseconds Coversion Time: " + tmpTimer.ElapsedMilliseconds);
            Console.ReadLine();
        }

    }
}
