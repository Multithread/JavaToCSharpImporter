using Multithreading;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JavaToCSharpConverter.Model
{
    public class ReplaceJob : Aufgabe
    {
        public ReplaceInFile ReplaceFile { get; set; }
        public override object Abarbeiten()
        {
            ReplaceFile.Search();
            return ReplaceFile;
        }
    }
}
