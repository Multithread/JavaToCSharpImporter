using JavaToCSharpConverter.Model;
using Multithreading;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
namespace JavaToCSharpConverter
{
    public class ReplaceHelper
    {
        public static void Run()
        {

            var tmpPath = @"C:\Data\Lucene7\lucene";

            var tmpThreadQueue = new ThreadQueue(16);
            tmpThreadQueue.AlleAufgabenBeendetEvent += TmpThreadQueue_AlleAufgabenBeendetEvent;
            foreach (var tmpFile in Directory.EnumerateFiles(tmpPath, "*", SearchOption.AllDirectories))
            {
                var tmpJob = new ReplaceJob
                {
                    ReplaceFile = new ReplaceInFile
                    {
                        FilePath = tmpFile,
                        Replacelist = new List<SearchAndReplacePattern>
                        {
                            new SearchAndReplacePattern
                            {
                                SearchPattern=@"\(short\)stripOrd\.intValue\(\)",
                                ReplacePattern="(short)0/*(short)stripOrd.intValue()*/"
                            },
                            new SearchAndReplacePattern
                            {
                                SearchPattern=@"([ \(]){1,1}([a-zA-Z0-9]){1.30}\.intValue\(\)",
                                ReplacePattern="0/*Data.intValue()*/"
                            }
                        }
                    }
                };
                tmpThreadQueue.addAufgabe(tmpJob);
            }
        }

        private static void TmpThreadQueue_AlleAufgabenBeendetEvent(object sender, Event e)
        {
            ResultCache.Bag.ToList().ForEach(inItem => Console.WriteLine(inItem));
            Console.ReadLine();
        }
    }
}
